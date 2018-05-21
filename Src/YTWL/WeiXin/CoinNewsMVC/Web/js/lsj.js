$.loading = function (bool, text) {
    var $loadingpage = parent.$("#loadingPage");
    var $loadingtext = $loadingpage.find('.loading-content');
    if (bool) {
        $loadingpage.show();
    } else {
        if ($loadingtext.attr('istableloading') == undefined) {
            $loadingpage.hide();
        }
    }
    if (!!text) {
        $loadingtext.html(text);
    } else {
        $loadingtext.html("数据加载中，请稍后…");
    }
    $loadingtext.css("left", (parent.$('body').width() - $loadingtext.width()) / 2 - 50);
    $loadingtext.css("top", (parent.$('body').height() - $loadingtext.height()) / 2);
}

$.dialog = function (msg,type)
{
    var content = '';
    if (type == 0) {
        content = '<button class="btn btn-success btn-circle btn-lg" type="button"><i class="fa fa-check"></i></button><span class="dialog-content">'+ msg +'</span>'
    } else
    {
        content = '<button class="btn btn-danger btn-circle btn-lg" type="button"><i class="fa fa-times"></i></button><span class="dialog-content">'+ msg +'</span>'
    }
    var d = dialog({
        fixed: true,
        content: content,
        padding: 20

    });
    d.show();
    //关闭提示模态框
    setTimeout(function () {
        d.close().remove();
    }, 2000);
}