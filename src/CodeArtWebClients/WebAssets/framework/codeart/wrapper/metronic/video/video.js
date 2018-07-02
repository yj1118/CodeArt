$$.createModule("Wrapper.metronic.video", function (api, module) {
    api.requireModules(["Wrapper.metronic"]);
    api.requireModules(["Component"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;
    var $o = util.object, $vir = $o.virtual, $component = $$.component;

    var $video = $$.wrapper.metronic.video = function (painter, validator) {
        $o.inherit(this, $component.ui, painter, validator);
        var my = this;
        my.give = function (o) {
            $o.callvir(this, $component.ui, "give", o);

            o.mode = function (v) {
                if (v) {
                    this.getJquery().css("height", "auto");
                    if (v == 'youku') {
                        this.jpC().hide();
                        this.youkuC().show();
                    } else {
                        this.jpC().show();
                        this.youkuC().hide();
                    }

                    this.__mode = v;
                }
                return this.__mode;
            }

            o.jpC = function () {
                var id = o.para("id");
                return $("#jp_container_" + id);
            }

            o.youkuC = function () {
                var id = o.para("id");
                return $("#youku_container_" + id);
            }

            o.player = function () {
                var id = this.para("id");
                if (this.mode() == 'youku') {
                    return this.__youkuPlayer;
                }
                return $("#playerArea_" + id);
            }

            o.start = function (p) {
                if (this.mode() == 'youku') {
                    var playerDom = $("#youku_player_" + this.para('id'));
                    playerDom.css("width", "100%");
                    playerDom.height(this.para("height"));
                    var player = new YKU.Player('youku_player_' + this.para('id'), {
                        styleid: '0',
                        client_id: p.client_id,
                        vid: p.vid,
                        autoplay: true,
                        show_related: false,
                        events: {
                            onPlayerReady: function () { /*your code*/ },
                            onPlayStart: function () { /*your code*/ },
                            onPlayEnd: function () { /*your code*/ }
                        }
                    });
                    this.__youkuPlayer = player;
                } else {
                    this.player().jPlayer("setMedia", p);
                    this.player().jPlayer("play");
                }
            }

            o.stop = function () {
                var m = this.mode();
                if (m == "youku") {
                    try {
                        this.player().pauseVideo();
                    } catch (e) { }
                }
                else {
                    this.player().jPlayer("stop");
                }
            }
            init(o);
        }

        function init(o) {
            var p = o.player();
            p.jPlayer({
                solution: "html,flash",//定义html和flash解决方案的优先级。默认优先使用html，flash备用。
                swfPath: "/js/jplayer/jquery.jplayer.swf",// 定义jPlayer 的Jplayer.swf文件的路径。
                supplied: "m4v,ogv,webmv",//定义提供给jPlayer的格式。顺序表示优先级，左边的格式优先级最高，右边的优先级较低。
                //M4A / M4V
                //MP4文件是一个同时支持音频和视频的容器。M4A是音频MP4, M4V是视频mp4. 推荐的标准浏览器和手机浏览器编码选项有：
                //H.264基线规范3.0级视频，30帧每秒时不小于640*480 ，注意基线规范不支持B帧。
                //AAC-LC音频，大于48kHz。
                size: {//设置restored screen 模式下的尺寸。默认尺寸取决于提供的是音频还是视频格式。当两者都提供时默认使用视频的默认尺寸。
                    width: "100%",
                    height: o.para("height"),
                    cssClass: "jp-video-auto"
                },
                useStateClassSkin: true,//控制播放和暂停是否一个按键
                autoBlur: false,
                smoothPlayBar: true,//平滑过渡播放条 Smooths the play bar transitions.播放条的变化是在250ms内动画效果平滑过渡，而不是一步完成的。播放条移动到了新的位置， 这也影响了它的点击。时长短的多媒体效果最明显，因为他们的变化步骤最大化了。
                keyEnabled: false,//是否启动快捷键
                remainingDuration: true,//显示剩余时间
                toggleDuration: true,//剩余时间与总时间切换
                cssSelectorAncestor: "#jp_container_" + o.para("id")
            });
            o.getJquery().find(".jp-video-play").css({
                height: o.para("height"),
                marginTop: "-" + o.para("height")
            });
            o.getJquery().find(".jp-video-play-icon").css({
                top: (parseInt(o.para("height")) / 2 - 50) + "px"
            });

        }
    }

});