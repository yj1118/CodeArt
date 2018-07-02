$$.createModule("Component.pageTitle", function (api, module) {
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component;

    var $pageTitle = $component.pageTitle = function (painter) {
        $o.inherit(this, $component.ui, painter);
        var my = this;
        this.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);
            o.back = function () {
                var ref = document.referrer;
                if (ref) history.go(-1);
                else window.close();

                //if (window.WeixinJSBridge) WeixinJSBridge.call("closeWindow");
            }
            init(o);
        }

        function init(o) {
            var a = o.find("a"),url=o.para("url");

            var ref = document.referrer;
            if (ref) {
                a.off("click.pageTitle");
                a.on("click.pageTitle", { o: o }, function (e) {
                    if (url) location.href = url;
                    else e.data.o.back();
                });
            }
            else {
                a.hide();
            }
        }
    }

    $pageTitle.painter = function (methods) {
        $o.inherit(this, $$.component.painter, methods);
    }

    $pageTitle.create = function () {
        return new $component.pageTitle(new $pageTitle.painter());
    }


});