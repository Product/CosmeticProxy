//获取产品列表
$(function () {
    $("#btnSearch").click(function() { /* 点击查询按钮 */
        getlist($("#categoryId").val(), $("#searchContent").val());
    });
    getlist(0,"");
    //列表获取
    function getlist(area,search) {

        $("#js_tbSzList").ajaxPage({
            url: "/Product/GetProductList?area=" + area + "&search=" + search,
            pageObj: $("#DataTables_Table_0_paginate"),
            pageIndex: 1,
            pageSize: 10,
            curPageCls: "paginate_active",
            pageInfo: {
                obj: $("#DataTables_Table_0_info"),
                content: "共{allCount}条  当前第{pageIndex}/{allPage}页"
            },

            dataFn: function (data, $this) {
                //var data = $.parseJSON(data);
                var dataHtml = "", dCont = data.data;

                if (dCont) {

                    var formatJsonDateStr = function (jsonDateStr) {
                        var date = new Date(parseInt(jsonDateStr.replace("/Date(", "").replace(")/", ""), 10));
                        var dateStr = [];
                        function toD(n) {
                            if (n < 10) {
                                return '0' + n;
                            } else {
                                return n;
                            }
                        }
                        dateStr.push(date.getFullYear());
                        dateStr.push('-' + toD((Number(date.getMonth()) + 1)));
                        dateStr.push('-' + toD(date.getDate()));
                        dateStr.push(' ' + toD(date.getHours()));
                        dateStr.push(':' + toD(date.getMinutes()));
                        dateStr.push(':' + toD(date.getSeconds()));
                        return dateStr.join('');
                    };


                    for (var i = 0; i < dCont.length; i++) {             
                        dataHtml += '' +
                            '<li>' +
                            '    <a hrerf="' + dCont[i].Code + '">' +//
                            '        <img alt="' + dCont[i].Name + '" src=http://www.cosmeticproxy.com/Product/GetPic/' + dCont[i].Pic + '/1/>' +//
                            '        <h4>' + dCont[i].Name + '</h4>' +//
                            '        <p>' + dCont[i].Brank + '</p>' +//
                            '        <p>' + dCont[i].Price + '</p>' +//
                            '    </a>' +//
                            '</li>';
                    }

                    $("#scrollable_one").show();
                    $("#js_tbSzList").empty().html(dataHtml);
                    return dataHtml;
                } else {
                    alert("暂无记录")
                    $("#js_tbSzList").empty()
                    $("#scrollable_one").hide();
                }
            }

        });
    }
});