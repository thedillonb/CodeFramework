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

function invokeNative(functionName, args) {
    var iframe = document.createElement('IFRAME');
    iframe.setAttribute('src', 'app://' + functionName + '#' + JSON.stringify(args));
    document.documentElement.appendChild(iframe);
    iframe.parentNode.removeChild(iframe);
    iframe = null;  
}

function addComments(comments) {
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

$(function() {
    invokeNative("ready");
});