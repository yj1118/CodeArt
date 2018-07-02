$(document).ready(function () {
    //由于$$view所处的模块是在document加载完毕后才能使用，所以在此处写
    $$view.callback = function (d) {//回调一个视图数据，在metronic环境下用page的ready方法注册回调函数
        $$.page.ready(function () { $$view.exec(d); });
    }
});