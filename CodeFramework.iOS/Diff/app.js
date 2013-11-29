function diff(base_text, new_text) {
    var base = difflib.stringAsLines(base_text);
    var newtxt = difflib.stringAsLines(new_text);
    var sm = new difflib.SequenceMatcher(base, newtxt);
    var opcodes = sm.get_opcodes();
    var diffoutputdiv = document.body;
    while (diffoutputdiv.firstChild) diffoutputdiv.removeChild(diffoutputdiv.firstChild);
    diffoutputdiv.appendChild(diffview.buildView({
        baseTextLines: base,
        newTextLines: newtxt,
        opcodes: opcodes,
        baseTextName: "Base Text",
        newTextName: "New Text",
        contextSize: 3,
        viewType: 1
    }));
    
    $('td').each(function(i, el) {
        $(el).click(function() {
            invokeNative("comment", {"line_to": $(el).parent().data('to'), "line_from": $(el).parent().data('from')});
        });
    });
}

function loadFileAsPatch(path) {
	$.get(path, function(data) {
		patch(data);
	});
}

function patch(p) {
	var $body = $('body');
	var $table = $("<table class='diff inlinediff'></table>");

	function createRow(x, y, type, line, lineNum) {
		$table.append("<tr data-to='" + lineNum + "' data-x='" + x + "' data-y='" + y + "'><th>" + x + "</th><th>" + y + "</th><td class='" + type + "'>" + line + "</td></tr>");
	};
	
	var lines = p.split("\n");
	var baseLine = 0;
	var newLine = 0;
	for (var i = 0; i < lines.length; i++) {
		var line = lines[i];
		if (line.lastIndexOf("@@", 0) === 0) {
			createRow("...", "...", "skip", line, i);
			var r = /@@ -(\d+).+\+(\d+)/i;
			var arr = r.exec(line);
			baseLine = arr[1];
			newLine = arr[2];
		} else {
			if (line.lastIndexOf("+", 0) === 0) {
				createRow("", newLine, "insert", line, i);
				newLine++;
			} else if (line.lastIndexOf("-", 0) === 0) {
				createRow(baseLine, "", "delete", line, i);
				baseLine++;
			} else {
				createRow(baseLine, newLine, "equal", line, i);
				baseLine++;
				newLine++;
			}
		}
	}
	
	$body.append($table);
	
    $('td:not(.skip)').each(function(i, el) {
        $(el).click(function() {
        	var fileLine = $(el).parent().data('y');
        	if (fileLine === "")
        		fileLine = $(el).parent().data('x')
            invokeNative("comment", {"patch_line": $(el).parent().data('to'), "file_line": fileLine});
        });
    });
}

function invokeNative(functionName, args) {
    try
    {
	    var iframe = document.createElement('IFRAME');
	    iframe.setAttribute('src', 'app://' + functionName + '#' + JSON.stringify(args));
	    document.body.appendChild(iframe);
	    iframe.parentNode.removeChild(iframe);
	    iframe = null;  
    }
    catch (err)
    {
    	alert(err.message);
    }
}

function setComments(comments) {

    $('tr.comment').remove();

    for (var i = 0; i < comments.length; i++) {
        var comment = comments[i];
        var $comment = $("<tr data-id='" + comment.Id + "' class='comment'><td colspan='3'><div class='inner'><header><img src='" + comment.Avatar + "' />" + comment.User + "</header><div class='content'>" + comment.Content + "</div></div></td></tr>");
        
        if (comment['LineTo'] != null) {
            $("tr[data-to='" + comment.LineTo + "']").after($comment);
        }
        else if (comment['LineFrom'] != null) {
            $("tr[data-from='" + comment.LineFrom + "']").after($comment);
        }
        else if (comment['Parent'] != null) {
            $("tr[data-id='" + comment.Parent + "']").after($comment);
        }
    }
}

window.onload = function() { document.location.href = 'app://ready'};
