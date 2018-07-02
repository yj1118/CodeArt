
tinymce.PluginManager.add('disk', function (editor) {

    //function disk_onMessage(event){
    //	if(editor.settings.external_filemanager_path.toLowerCase().indexOf(event.origin.toLowerCase()) === 0){
    //	    if (event.data.sender === 'disk') {
    //			tinymce.activeEditor.insertContent(event.data.html);
    //			tinymce.activeEditor.windowManager.close();

    //			// Remove event listener for a message from ResponsiveFilemanager
    //			if(window.removeEventListener){
    //				window.removeEventListener('message', disk_onMessage, false);
    //			} else {
    //				window.detachEvent('onmessage', disk_onMessage);
    //			}
    //		}
    //	}
    //}

    function openDisk() {
        editor.focus(true);
        var title = editor.settings.disk_title || "请选择文件";
        var editorId = editor.settings.editor_id;
        var diskId = "editor-disk-" + editorId;
        var modalId = "editor-disk-modal-" + editorId;

        var disk = $$("#" + diskId);
        if (!disk.getEvent("select")) {
            disk.on("select", function (k, d, item) {
                $("#" + modalId).modal('toggle');
                var s = d.source, c;
                if (d.isImg) c = "<img src=\"" + s + "\" />";
                else c = "<a href=\"" + s + "\" target='_blank'>" + d.name + "</a>";
                editor.insertContent(c);
            });
        }

        $$("#" + modalId).title(title);
        $("#" + modalId).modal('toggle');
    }

    editor.addButton('disk', {
        icon: 'browse',
        tooltip: '插入文件',
        shortcut: 'Ctrl+E',
        onclick: openDisk
    });

    editor.addShortcut('Ctrl+E', '', openDisk);

    editor.addMenuItem('disk', {
        icon: 'browse',
        text: '插入文件',
        shortcut: 'Ctrl+E',
        onclick: openDisk,
        context: 'insert'
    });
});