1.文件/assets/vendors/base/vendors.bundle.js被改动，注释了加载jquery-mousewheel的代码，因为这段代码里用的国外的CDN服务
我们单独下载了jquery-mousewheel.js文件
2.文件/assets/demo/default/base/scripts.bundle.js被改动，注释了1026行的代码$(window).resize(Plugin.fullRender);
这主要是为了解决datatable在窗口大小发生改变时闪烁的问题
以上两个问题在新版中metronic已经自己解决了，不需要我们修正