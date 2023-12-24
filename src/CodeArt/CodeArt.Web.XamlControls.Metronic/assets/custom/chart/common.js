﻿$$.createModule("metronic.chart", function (api, module) {
    api.requireModules(["metronic"]);

    var J = jQuery, util = api.util, type = util.type, empty = util.empty, getProxy = util.getProxy;

    var $chart = $$.metronic.chart = function (painter, validator) {
        var my = this;

        my.give = function (o) {
            var my = this;

            o.set = function (data) {
                var p = this.initOption(data);
                this._chart.setOption(p);
            }

            o.setOption = function (p) {
                if (this.initOption) {
                    this.initOption(p);
                }
                this._chart.setOption(p);
            }
            o.setPie = function (config) { //饼状图
                var option = getPieOption(config);
                this.setOption(option);
            }
            o.setLine = function (config) { //折线图
                var option = getLineOption(config);
                this.setOption(option);
            }
            o.setScatter = function (config) {
                var option = getScatterOption(config);
                this.setOption(option);
            }

            o.init = function () {
                var o = this, oj = this.getJquery();
                // 基于准备好的dom，初始化echarts实例
                if (o._chart) {
                    o._chart.dispose();
                }
                o._chart = echarts.init(this.ent());
                var p = oj.parent();
                var my = this;
                p.resize(function () {
                    my._chart.resize();
                });

                var option = oj.attr("option") || oj.attr("data-option");
                if (option) {
                    eval("o.initOption=" + option+";");
                }
            }
            if (o.attr("data-lazyInit") !== "true") //在bind数据或克隆模式下，需要手工初始化
                o.init();
        }



        function getPieOption(config) {
            var g = config;
            if (!g.title) g.title = {};
            if (!g.series) g.series = {data:[]};
            var legendData = g.series.data.select((t) => t.name);
            return {
                title: {
                    text: g.title.text || '',
                    subtext: g.title.subtext || '',
                    x: g.title.x || 'left'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    type: 'scroll',
                    orient: 'vertical',
                    right: 0,
                    top: 0,
                    bottom: 0,
                    data: legendData
                },
                series: [
                    {
                        name: g.series.name||'',
                        type: 'pie',
                        radius: '65%',
                        center: ["40%","50%"],
                        data: g.series.data,
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };

        }

        function getLineOption(config) {
            var g = config;
            if (!g.title) g.title = {};
            if (!g.series) g.series = [];
            if (!g.xAxis) g.xAxis = { data: [] };

            var fm = _formatter[g.formatter];
            if (fm) g.xAxis.data = fm.xDatas(g.xAxis.data);

            g.series.each(function (i) {
                this.type = "line";
                this.label = {
                    normal: {
                        show: true,
                        position: 'top',
                        formatter: fm ? fm.yAxisLabel : null
                    },
                };
                var col = getColor(i);
                this.lineStyle = { normal: { color: col.master } };
                this.itemStyle = { normal: { color: col.master } };
                //this.areaStyle = { normal: { color: col.slave } };
                if (fm) fm.data(this, g.xAxis.data);
            });

            var legendData = g.series.select((t) => t.name);
            var option = {
                title: {
                    text: g.title.text || '',
                    subtext: g.title.subtext || '',
                    x: g.title.x || 'left'
                },
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'cross',
                        label: {
                            backgroundColor: '#6a7985',
                            formatter: fm ? fm.tooltipLabel : null
                        }
                    },
                    formatter: fm ? fm.tooltip : null,
                },
                legend: {
                    data: legendData
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                xAxis: {
                    boundaryGap: true, //不靠边界
                    data: g.xAxis.data,
                    splitLine: {
                        show: false
                    }
                },
                yAxis: {
                    type: 'value',
                    axisLabel: {
                        formatter: fm ? fm.yAxisLabel : null
                    },
                    splitLine: {
                        show: false
                    }
                },
                series: g.series
            };

            if (fm) {
                fm.range(option);
            }

            return option;
        }

        function getScatterOption(config) {
            var g = config;
            if (!g.title) g.title = {};
            if (!g.series) g.series = [];
            if (!g.symbolSize) g.symbolSize = 30;
            if (!g.nameFontSize) g.nameFontSize = 20;

            var data = g.series.select((t) => [t.x || t.X || 0, t.y || t.Y || 0,t.name||t.Name||'']);
            var option = {
                title: {
                    text: g.title.text || '',
                    subtext: g.title.subtext || '',
                    x: g.title.x || 'left'
                },
                xAxis: g.xAxis||{},
                yAxis: g.yAxis||{},
                series: [{
                    label: {
                        normal: {
                            show: true,
                            formatter: function (param) {
                                return param.data[2];
                            },
                            position: 'top',
                            textStyle: {
                                fontSize: g.nameFontSize
                            }
                        }
                    },
                    itemStyle: {
                        normal: {
                            shadowBlur: 10,
                            shadowColor: 'rgba(120, 36, 50, 0.5)',
                            shadowOffsetY: 5,
                            color: new echarts.graphic.RadialGradient(0.4, 0.3, 1, [{
                                offset: 0,
                                color: 'rgb(251, 118, 123)'
                            }, {
                                offset: 1,
                                color: 'rgb(204, 46, 72)'
                            }])
                        }
                    },
                    symbolSize: 30,
                    data: data,
                    type: 'scatter'
                }]
            };
            return option;
        }


        var _formatter = {
            hhmm: {
                data: function (s, xDatas) { //更改数据的值
                    var ds = [];//以下代码是根据x值，找到对应的y轴值，如果找不到，那么值为0
                    xDatas.each(function () {
                        var yMd = this;
                        var d = s.data.first(function () {
                            return this.format("y/M/d") === yMd;
                        });
                        ds.push(d || 0);
                    });
                    s.data = ds.select((v) => v !== 0 ? getDayMinutes(v) : 0);
                },
                tooltip: function (ps) {
                    if (ps.length === 0) return;
                    var s = [], f = ps[0];
                    s.push(f.name);
                    ps.each(function () {
                        var p = this, ms = p.data, n = p.seriesName;
                        s.push(p.marker+n + ": " + hhmmText(ms));
                    });
                    return s.join("</br>");
                },
                tooltipLabel: function (v) {
                    if (util.type(v.value) === "number") return hhmmText(v.value);
                    return v.value;
                },
                xDatas: function (xDatas) {
                    return xDatas.select((d) => d.format("y/M/d")).distinct();
                },
                yAxisLabel: function (v) {
                    v = empty(v.value) ? v : v.value;
                    return hhmmText(v);
                },
                range: function (p) { //更改范围
                    p.yAxis.min = 0;
                    p.yAxis.max = 24 * 60;
                },
            }
        };

        function getDayMinutes(d) { //获取日期当天的总分钟
            return d.getHours() * 60 + d.getMinutes();
        }

        function hhmmText(ms) { //根据当天的总分钟数，得到hh:mm的数值
            var m = ms % 60, h = (ms - m) / 60;
            return (parseInt(h) + '').pad(2) + ":" + (parseInt(m) + '').pad(2);
        }

        var _color = [
            { master: '#0dc8de', slave: '#a7ebf3' },
            { master: '#e13a58', slave: '#e64d81' },
            { master: '#96b4d3', slave: '#b3d1f0' },
            { master: '#67b7dc', slave: '#d2eaf5' },
            { master: '#cea6a6', slave: '#f0c2c2' },
        ];

        function getColor(i) {
            var t = i % _color.length;
            return _color[t];
        }
    }
});