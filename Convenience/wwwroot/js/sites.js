// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// アコーデオンの状態を制御する関数
function AccordionHandller() {

    // 初期画面の場合は、アコーデオンは開いた状態にする
    // その判断は、ControllerからViewBagに設定してもらう
    if ($('#handlingFlg').data("message") == "FirstDisplay") {
        console.log($.cookie("AccordionOpenStatus"));

        // 既存のアコーデオン状態のクッキーを削除
        document.cookie = "cookieAccordionOpenStatus=; expires=Thu, 01 Jan 1970 00:00:00 UTC";

        // アコーデオン状態を"Show"として設定
        $.cookie("AccordionOpenStatus", "Show");

        console.log($.cookie("AccordionOpenStatus"));
    }

    // アコーデオン制御
    let accordion = document.getElementById('accordion');

    // クッキーが"Show"の場合、アコーデオンを開く
    if ($.cookie("AccordionOpenStatus") == "Show") {
        let activeCollapse = accordion.querySelector('.accordion-collapse');
        if (activeCollapse) {
            activeCollapse.classList.add('show');
        }
    }
    // クッキーが"Hide"の場合、アコーデオンを閉じる
    else if ($.cookie("AccordionOpenStatus") == "Hide") {
        let activeCollapse = accordion.querySelector('.accordion-collapse.show');
        if (activeCollapse) {
            activeCollapse.classList.remove('show');
        }
    }

    // コンテンツ表示
    $('#content').show();

    // アコーデオンリサイズ
    let breakpoint = 576; // 閉じるブレークポイント

    // ブラウザのサイズを変えられた時にコールされる
    function closeAccordionIfSmallScreen() {
        // スマホサイズになったら
        if (window.innerWidth <= breakpoint) {
            // アコーデオンが開いている状態か
            let activeCollapse = accordion.querySelector('.accordion-collapse.show');
            if (activeCollapse) {
                // アコーデオンを閉じる
                activeCollapse.classList.remove('show');
                // 閉じた状態であることをクッキーに保存
                $.cookie("AccordionOpenStatus", "Hide");
            }
        }
        // スマホサイズを超えたら
        if (window.innerWidth > breakpoint) {
            // アコーデオンが閉じている状態か
            let activeCollapse = accordion.querySelector('.accordion-collapse');
            if (activeCollapse) {
                // アコーディオンを開く
                activeCollapse.classList.add('show');
                // 開いた状態であることをクッキーに保存
                $.cookie("AccordionOpenStatus", "Show");
            }
        }
    }

    // リサイズ時にアコーデオンを制御するイベントリスナー設定
    $(window).on('resize', function () {
        closeAccordionIfSmallScreen();
    });

    // 店頭在庫検索時、アコーデオンの状態を更新する
    $('#need-collapse-at-retrival').on('submit', function () {
        if (window.innerWidth <= breakpoint) {
            $.cookie("AccordionOpenStatus", "Hide", { path: '/' });
        } else {
            $.cookie("AccordionOpenStatus", "Show", { path: '/' });
        }
    });

    // アコーデオンが開かれたときの処理
    $('#accordion').on('show.bs.collapse', function () {
        $.cookie("AccordionOpenStatus", "Show", { path: '/' });
    });

    // アコーデオンが閉じられたときの処理
    $('#accordion').on('hide.bs.collapse', function () {
        $.cookie("AccordionOpenStatus", "Hide", { path: '/' });
    });
}

// 連打防止および入力フォームの動作を制御する関数
function RendaSolution() {
    // 連打対応
    $('input').on('focus', function () {
        $('#remark').empty();
    });

    // readonlyが設定されているカスタム要素に対する処理
    $('.custom-disabled').on('focus', function () {
        $(this).prop('readonly', true);
    });

    $('.custom-disabled').on('blur', function () {
        $(this).prop('readonly', false);
    });

    $('#submit_btn').on('click', function (event) {
        event.preventDefault(); 
        $(this).prop("disabled", true);
        $(this).closest('form').submit();
    });
}

// 最初にフォーカスを指定した位置に移動させる関数
function FirstFocus(firstPosition) {

    // 指定した位置にフォーカスを当てる処理
    if (firstPosition && firstPosition.trim() !== "") {
        $(firstPosition).focus();
        $(firstPosition).select();
    };

}

// 商品名称を取得する関数（会計入力用）
function GetShohinName() {

    // 商品IDの要素を取得
    var shohinIdElement = $("#KaikeiJissekiforAdd_ShohinId");
    if (shohinIdElement.length > 0) {  // 要素が存在する場合のみ
        GetShohinNameWithAjax(shohinIdElement);
    }

    // ShohinIdの選択が変更されたときにイベントを発火
    $('#KaikeiJissekiforAdd_ShohinId').change(function () {
        GetShohinNameWithAjax(this);
        $('#KaikeiJissekiforAdd_UriageSu').focus();     // 売上数項目にジャンプ
        $('#KaikeiJissekiforAdd_UriageSu').select();    // テキストを選択
    });

}

// Ajaxで商品名を取得する関数（会計入力用）
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

// フォームをリセットする関数
function ResetForm(stringForm) {
    // 指定されたフォームをリセット
    document.getElementById(stringForm).reset();
}

// 戻るボタンを無効化する関数
function PreventModoru() {
    // 戻るボタンが押されたときにアラートを表示
    window.onpopstate = function (event) {
        alert("このページでは戻るボタンは無効です。");
        history.pushState(null, null, location.href); // 現在のURLを再度プッシュ
    };

    // 初期状態として履歴に現在のページをプッシュ
    history.pushState(null, null, location.href);
}
