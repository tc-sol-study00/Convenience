// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function AccordionHandller() {

    //初期画面の場合は、アコーデオンは開いた状態にする
    //その判断は、ContollerからViewBagに設定してもらう
    if ($('#handlingFlg').data("message") == "FirstDisplay") {
        $.cookie("AccordionOpenStatus", "Show");
    }
    //アコ―デオン制御
    let accordion = document.getElementById('accordion');

    if ($.cookie("AccordionOpenStatus") == "Show") {
        let activeCollapse = accordion.querySelector('.accordion-collapse');
        if (activeCollapse) {
            activeCollapse.classList.add('show');
        }
    } else if ($.cookie("AccordionOpenStatus") == "Hide") {
        let activeCollapse = accordion.querySelector('.accordion-collapse.show');
        if (activeCollapse) {
            activeCollapse.classList.remove('show');
        }
    }
    //画面を表示する
    $('#content').show();

    //アコ―デオンリサイズ
    let breakpoint = 576; // 閉じるブレークポイント

    //ブラウザの大きさを変えられた時にコールされる
    function closeAccordionIfSmallScreen() {
        //スマホサイズになったら
        if (window.innerWidth <= breakpoint) {
            //アコーデオンが開いている状態か
            let activeCollapse = accordion.querySelector('.accordion-collapse.show');
            if (activeCollapse) {
                //アコーデオンを閉じる
                activeCollapse.classList.remove('show');
                //閉じた状態であることをクッキーに保存
                $.cookie("AccordionOpenStatus", "Hide");
            }
        }
        //スマホサイズを超えたら
        if (window.innerWidth > breakpoint) {
            //アコーデオンが閉じている状態か
            let activeCollapse = accordion.querySelector('.accordion-collapse');
            if (activeCollapse) {
                //アコーディオンを開く
                activeCollapse.classList.add('show');
                //開いた状態であることをクッキーに保存
                $.cookie("AccordionOpenStatus", "Show");
            }
        }
    }

    //リスナー設定

    //リサイズされたとき
    $(window).on('resize', function () {
        closeAccordionIfSmallScreen();
    });

    //アコーデオンが開かれたとき
    $('#accordion').on('show.bs.collapse', function () {
        $.cookie("AccordionOpenStatus", "Show");
    });

    //アコーデオンが閉じられた時
    $('#accordion').on('hide.bs.collapse', function () {
        $.cookie("AccordionOpenStatus", "Hide");
    });
}
function RendaSolution() {
    //連打対応
    $('input').on('focus', function () {
        $('#remark').empty();
    });
    $('.custom-disabled').on('focus', function () {
        $(this).prop('readonly', true);
    });

    $('.custom-disabled').on('blur', function () {
        $(this).prop('readonly', false);
    });
}

function FirstFocus(firstPosition) {

    if (firstPosition && firstPosition.trim() !== "") {
        $(firstPosition).focus();
        $(firstPosition).select();
    };

}
function GetShohinName() {

    var shohinIdElement = $("#KaikeiJissekiforAdd_ShohinId");  // 引数で渡されたIDの要素を取得
    if (shohinIdElement.length > 0) {  // 要素が存在する場合のみ
        GetShohinNameWithAjax(shohinIdElement);
    }

    // ShohinIdの選択が変更されたときにイベントを発火
    $('#KaikeiJissekiforAdd_ShohinId').change(function () {
        GetShohinNameWithAjax(this);
        $('#KaikeiJissekiforAdd_UriageSu').focus();     //売上数項目にジャンプ
        $('#KaikeiJissekiforAdd_UriageSu').select();    // テキストを選択
    });

}

function GetShohinNameWithAjax(item) {

    // CSRFトークンをAJAXリクエストのヘッダーに追加
    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        }
    });
    var selectedShohinId = $(item).val(); // 選択された商品IDを取得

    $.ajax({
        url: '/Kaikei/GetShohinName', // ASP.NET Coreのエンドポイント
        type: 'POST',                    // メソッドタイプはPOST
        data: {
            ShohinId: selectedShohinId    // 選択された商品IDをサーバーに送信
        },
        success: function (response) {
            // 成功時、ShohinNameのテキストフィールドに取得した商品名をセット
            $('#KaikeiJissekiforAdd_ShohinName').val(response);

        },
        error: function () {
            // エラー時、空の値をセット
            $('#ShohinName').val('商品名が見つかりません');
        }
    });
}

function ResetForm(stringForm) {
    document.getElementById(stringForm).reset();  // フォームをリセット
}

function PreventModoru() {
    window.onpopstate = function (event) {
        // 戻るボタンが押されたとき
        alert("このページでは戻るボタンは無効です。");
        history.pushState(null, null, location.href); // 現在のURLを再度プッシュ
    };

    // 初期状態として履歴に現在のページをプッシュ
    history.pushState(null, null, location.href);
}