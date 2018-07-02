
tinymce.PluginManager.add('insertCode', function (editor) {

    var _langs= ['c#', 'c', 'java', 'php', 'css', 'javascript', 'html', 'as3', 'vb', 'xml', 'xslt',
                    'applescript', 'bash', 'coldfusion', 'delphi', 'diff', 'erl', 'groovy', 'javafx', 'perl',
                    'plain', 'python', 'ruby', 'sass', 'scala', 'sql'];


    function showDialog(linkList) {
        var data = {}, selection = editor.selection, dom = editor.dom, selectedElm, anchorElm, initialText, initialLang;
        var win, langs;


        selectedElm = selection.getNode();
        anchorElm = dom.getParent(selectedElm, 'pre');
        initialText = anchorElm ? (anchorElm.innerText || anchorElm.textContent) : selection.getContent({ format: 'text' });
        initialLang = anchorElm ? dom.getAttrib(anchorElm, 'class') : '';
        initialLang = initialLang ? initialLang.split(' ')[1].replace(';', '') : _langs[0];


        function insertCode() {
            var lang = win.find('#lang')[0].value();
            var code = win.find('#code')[0].value();

            var html = ['<div class=\"sourceCode\"><pre class="brush: ', lang, ';">', code, '</pre></div><p></p>'].join('');
            var initialHtml = ['<div class=\"sourceCode\"><pre class="brush: ', initialLang, ';">', initialText, '</pre></div><p></p>'].join('');

            if (html !== initialHtml) {
                if (anchorElm) {
                    editor.focus();
                    anchorElm.innerHTML = code;

                    dom.setAttribs(anchorElm, {
                        'class': ["brush: ", lang, ";"].join('')
                    });

                    selection.select(anchorElm);
                } else {
                    editor.execCommand('mceInsertContent', false, html);
                }
            }
        }
       
        

        function buildLangs(targetValue) {
            var items = [], l = _langs;
            for (var i = 0, len = l.length; i < len; i++) {
                var c=l[i];
                items.push({ text: c, value: c });
            }

            return items;
        }

        win = editor.windowManager.open({
            title: 'Insert Code',
            data: data,
            body: [
                {
                    type: 'container', label: 'Programming Language', items: {
                            name: 'lang',
                            type: 'listbox',
                            values: buildLangs(data.target),
                            minWidth: 100,
                            value: initialLang
                        }
                },
                { label: 'Source Code', type: 'label', text: '\u8bf7\u5728\u4e0b\u9762\u7684\u6587\u672c\u6846\u4e2d\u8f93\u5165\u6216\u8005\u7c98\u8d34\u5bf9\u5e94\u7f16\u7a0b\u8bed\u8a00\u7684\u6e90\u4ee3\u7801' },
				{
				    name: 'code',
				    type: 'textbox',
				    multiline: true,
				    minWidth: 800,
				    minHeight: 500,
				    value: initialText
				}
            ],
            onsubmit: function () {
                insertCode();
            }
        });
    }


    function createCode(callback) {
    	return function() {
    	    callback();
    	};
    }

    editor.addButton('insertCode', {
        text: 'Insert Code',
        tooltip: 'Insert Or Edit Code',
        onclick: createCode(showDialog)
    });

});