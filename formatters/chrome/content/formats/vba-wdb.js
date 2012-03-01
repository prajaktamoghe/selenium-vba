
this.name = "vba-wdb";

// Characters that should be escaped when saving.
var EncodeToXhtmlEntity = ["amp", "gt", "lt", "quot", "nbsp"];

var XhtmlEntityFromChars = {};
for (var i = 0; i < EncodeToXhtmlEntity.length; i++) {
    var entity = EncodeToXhtmlEntity[i];
    XhtmlEntityFromChars[XhtmlEntities[entity]] = entity;
}

// A regular expression that matches characters that can be converted to entities.
var XhtmlEntityChars = "[";
for (var code in XhtmlEntityFromChars) {
    var c = parseInt(code).toString(16);
    while (c.length < 4) {
        c = "0" + c;
    }
    XhtmlEntityChars += "\\u" + c;
}
XhtmlEntityChars += "]";

function decodeText(text) {
	text = text.replace(/""/g, '"');
	return text;
}

function encodeText(text) {
    if (text == null) return "";
	return text;
}

function convertText(command, converter) {
	var props = ['command', 'target', 'value'];
	for (var i = 0; i < props.length; i++) {
		var prop = props[i];
		command[prop] = converter(command[prop]);
	}
}

/**
 * Parse source and update TestCase. Throw an exception if any error occurs.
 *
 * @param testCase TestCase to update
 * @param source The source to parse
 */
function parse(testCase, source) {

	//var doc = new RegExp('selenium\\.start(.*)selenium\\.stop','g').exec(source)[1];  //source.substr(testIndex);
	var cmdStart = options["instance"] + ".start";
	var cmdStop = options["instance"] + ".stop";
	var startIndex = source.toLowerCase().indexOf( cmdStart.toLowerCase() );
	var stopIndex = source.toLowerCase().indexOf( cmdStop.toLowerCase() );
	if(startIndex==-1) throw cmdStart + " is missing !";
	if(stopIndex==-1) throw cmdStop + " is missing !";
	
	var doc = source.substr(startIndex + cmdStart.length, stopIndex - startIndex - cmdStart.length).replace('""', '¤');	
	
	var commandRegexp = new RegExp( options.commandLoadPattern.replace('instance', options["instance"] ));
	var commentRegexp = new RegExp(options.commentLoadPattern);
	var commandOrCommentRegexp = new RegExp("((" + options.commandLoadPattern.replace('instance', options["instance"] ) + ")|(" + options.commentLoadPattern + "))", 'g');

	var commands = [];
	var commandFound = false;
	var lastIndex;
	while (true) {
		lastIndex = commandOrCommentRegexp.lastIndex;
		var docResult = commandOrCommentRegexp.exec(doc);
		LOG.warn(docResult);
		if (docResult) {
			if (docResult[2]) { // command
				var command = new Command();
				command.skip = docResult.index - lastIndex;
				command.index = docResult.index;
				var result = commandRegexp.exec(doc.substring(lastIndex));
				eval(options.commandLoadScript);
				convertText(command, decodeText);
				command.command = command.command.substr(0, 1).toLowerCase() + command.command.substr(1);
				command.target = command.target.replace('¤', '""');
				command.value = command.value.replace('¤', '""');
				if(command.variable) {
					command.command = command.command.replace(/^get|is/, 'store');
					if(command.target != ''){
						command.value = command.variable;
					}else{
						command.target = command.variable;
					}
				}
				commands.push(command);
				if (!commandFound) {
					// remove comments before the first command or comment
					for (var i = commands.length - 1; i >= 0; i--) {
						if (commands[i].skip > 0) {
							commands.splice(0, i);
							break;
						}
					}
					commandFound = true;
				}
			} else { // comment
				var comment = new Comment();
				comment.skip = docResult.index - lastIndex;
				comment.index = docResult.index;
				var result = commentRegexp.exec(doc.substring(lastIndex));
				eval(options.commentLoadScript);
				commands.push(comment);
			}
		} else {
			break;
		}
	}
	if (commands.length > 0) {
		testCase.commands = commands;
	}else {
		throw "no command found";
	}
}

/**
 * Format an array of commands to the snippet of source.
 * Used to copy the source into the clipboard.
 *
 * @param The array of commands to sort.
 */
function formatCommands(commands) {
	var commandsText = '';
	for (i = 0; i < commands.length; i++) {
		var text = getSourceForCommand(commands[i]);
		commandsText = commandsText + text;
	}
	return commandsText;
}

function getSourceForCommand(commandObj) {
	var command = null;
	var comment = null;
	var template = '';
	if (commandObj.type == 'command') {
		command = commandObj;
		command = command.createCopy();
		convertText(command, this.encodeText);

		if(command.command.match(/^store/)){
			if(editor.seleniumAPI.Selenium.prototype[ command.command.replace(/^store/, 'is')]){
				template = options.commandTemplate.replace(/\$\{command.command\}/g, command.command.replace(/^store/, 'is') + '(' );
			}else if(editor.seleniumAPI.Selenium.prototype[ command.command.replace(/^store/, 'get')]){
				template = options.commandTemplate.replace(/\$\{command.command\}/g, command.command.replace(/^store/, 'get') + '(' );
			}else{
				template = options.commandTemplate.replace(/\$\{command.command\}/g, command.command + '(' );
			}
			if(command.value == ''){
				template = template.replace(/\$\{instance\}/g, command.target + " = " + options["instance"]);
				template = template.replace(/\$\{command.target\}/g, '' );
				template = template.replace(/\$\{command.value\}/g, ')' );				
			}else{
				template = template.replace(/\$\{instance\}/g, command.value + " = " + options["instance"]);
				template = template.replace(/\$\{command.target\}/g, '"' + command.target + '"' );
				template = template.replace(/\$\{command.value\}/g, ')' );	
			}
		}else{
			template = options.commandTemplate.replace(/\$\{instance\}/g, options["instance"]);
			template = template.replace(/\$\{command.command\}/g, command.command.replace(/^verify/, 'assert') );
			template = template.replace(/\$\{command.target\}/g, (command.target == '' ? '' : ' "' + command.target + '"') );
			template = template.replace(/\$\{command.value\}/g, (command.value == '' ? '' : ', "' + command.value + '"') );
		}
	} else if (commandObj.type == 'comment') {
		comment = commandObj;
		template = options.commentTemplate.replace(/\$\{comment.comment\}/g, comment );
	}
	return template;
}

/**
 * Format TestCase and return the source.
 * The 3rd and 4th parameters are used only in default HTML format.
 *
 * @param testCase TestCase to format
 * @param name The name of the test case, if any. It may be used to embed title into the source.
 * @param saveHeaderAndFooter true if the header and footer should be saved into the TestCase.
 * @param useDefaultHeaderAndFooter Parameter used for only default format.
 */
 
function format(testCase, name, saveHeaderAndFooter, useDefaultHeaderAndFooter) {
    var TcTitle = testCase.getTitle();
	if (TcTitle) {
		TCname=TcTitle;
	}
	
	var text;
	var commandsText = "";
	var testText;
	var i;
	
	for (i = 0; i < testCase.commands.length; i++) {
		var text = getSourceForCommand(testCase.commands[i]);
		commandsText = commandsText + text;
	}
	
	var testText;
	testText = options.testTemplate;
	testText = testText.replace(/\$\{name\}/g, TCname.replace(/\s/, '_')).
		replace(/\$\{baseURL\}/g, testCase.getBaseURL()).
		replace(/\$\{instance\}/g, options["instance"]).
		replace(/\$\{browser\}/g, options["browser"]);
		
	var commandsIndex = testText.indexOf("${commands}");
	if (commandsIndex >= 0) {
		var header = testText.substr(0, commandsIndex);
		var footer = testText.substr(commandsIndex + "${commands}".length);
		testText = header + commandsText + footer;
	}
	return testText;
}

/*
 * Optional: The customizable option that can be used in format/parse functions.
 */
 
this.options = {

	instance: "selenium",
	browser: "Browser_Chrome",

	commandLoadPattern:
	'((\\w+)\\s*=\\s*)?instance\\.(\\w+)([\\(\\s]\"([^\"]*)\"(\\,\\s*\"([^\"]*)\")?)?',
	
	commandLoadScript:
	"command.variable = result[2];\n" +
	"command.command = result[3];\n" +
	"command.target = result[5]||'';\n" +
	"command.value = result[7]||'';\n",

	commentLoadPattern:	"\\'(.+)\\n",
	commentLoadScript: "comment.comment = result[1];\n",

	testTemplate:
	'Public Sub ${name}()\n' +
	'  Dim ${instance} As New SeleniumWrapper.WebDriver\n' +
	'  ${instance}.start ${browser}, "${baseURL}"\n\n' +
	'${commands}\n'+
	'  ${instance}.stop\n' +
	"End Sub",

	commandTemplate: '  ${instance}.${command.command}${command.target}${command.value}\n',
	commentTemplate: "  '${comment.comment}\n",
	escapeDollar: "false"
};
	
this.configForm = 
	'<description>Instance</description>' +
	'<textbox id="options_instance" />' +
	'<description>Browser</description>' +
	'<textbox id="options_browser" />' +
	'<separator class="groove"/>' +
	'<description>Template for new test</description>' +
	'<textbox id="options_testTemplate" multiline="true" flex="1" rows="8"/>';
