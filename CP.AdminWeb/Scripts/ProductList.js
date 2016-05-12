//获取产品列表
$(function () {
    getlist();
    //列表获取
    function getlist() {

        $("#js_tbSzList").ajaxPage({
            url: "/Product/GetProductList",
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



                    var changeLinkText = function(obj) {
                        var link;
                        if(obj.Status == 0)
                            link = '<a id="DelProduct" code = "' + obj.Code + '">删除</a><br/><a id="UpdateStatus" code="' + obj.Code + '" status="' + obj.Status + '">发布</a><br/><a href=http://admin.cosmeticproxy.com/Product/EditProduct?code="' + obj.Code + '">编辑</a>';
                        if(obj.Status == 2)
                            link = '<a id="UpdateStatus" code="' + obj.Code + '" status="' + obj.Status + '">取消发布</a ><br/><a href="http://admin.cosmeticproxy.com/Product/EditProduct?code=' + obj.Code + '">编辑</a>';
                        return link;
                    }


                    for (var i = 0; i < dCont.length; i++) {             
                        dataHtml += '' +
                            '<tr>' +
                            '    <td>' + dCont[i].Code + '</td>' +//
                            '    <td width="100px">' + dCont[i].Name + '</td>' +//
                            '    <td>' + dCont[i].Brank + '</td>' +//
                            '    <td width="200px">' + dCont[i].Description + '</td>' +//
                            '    <td>' + dCont[i].Price + '</td>' +//
                            '    <td>' + dCont[i].Area + '</td>' +//
                            '    <td><img src=http://admin.cosmeticproxy.com/Product/GetPic/' + dCont[i].Pic + '/1/></td>' +//
                            '    <td>' + formatJsonDateStr(dCont[i].CreateTime) + '</td>' +//
                            '    <td>' + changeLinkText(dCont[i]) + '</td>' + //
                            '</tr>';
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