// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function AccordionHandller() {
    $(document).ready(function () {
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