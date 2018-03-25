1.由于原始库用CDN的方式引用了jquery.mousewheel.min.js，所以我们需要将jquery.mousewheel.min.js本地化，
需要把assets/vendors/base/vendors.bundle.js文件中关于jquery.mousewheel.min.js的外部引用代码删除,具体如下：
  function(init){
	var _rjs=typeof define==="function" && define.amd, /* RequireJS */
		_njs=typeof module !== "undefined" && module.exports, /* NodeJS */
		_dlp=("https:"==document.location.protocol) ? "https:" : "http:", /* location protocol */
		_url="cdnjs.cloudflare.com/ajax/libs/jquery-mousewheel/3.1.13/jquery.mousewheel.min.js";
	if(!_rjs){
		//if(_njs){
		//	require("jquery-mousewheel")($);
		//}else{
		//	/* load jquery-mousewheel plugin (via CDN) if it's not present or not loaded via RequireJS 
		//	(works when mCustomScrollbar fn is called on window load) */
		//	$.event.special.mousewheel || $("head").append(decodeURI("%3Cscript src="+_dlp+"//"+_url+"%3E%3C/script%3E"));
		//}
	}
	init();
}
所以在更新新版库的时候，也要在以上代码段中注释相关代码