// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function googleTranslateElementInit() {
    new google.translate.TranslateElement({ pageLanguage: 'en', layout: google.translate.TranslateElement.InlineLayout.SIMPLE }, 'google_translate_element');
}


//function changeLanguage(lang) {

//    if (lang === '/en/en') {
//        Cookies.remove('googtrans');
//    } else {
//        Cookies.set('googtrans', lang, { expires: 365 });
//    }
//    location.reload();
//}

//$(document).ready(function () {
//    var websiteLang = Cookies.get('googtrans');
//    if (websiteLang === '/en/ar') {
//        document.getElementById('engLang').classList.remove('d-none');
//        document.getElementById('arLang').classList.add('d-none');
//    } else {
//        document.getElementById('engLang').classList.add('d-none');
//        document.getElementById('arLang').classList.remove('d-none');
//    }
//});

$(document).ready(function () {
    var websiteLang = Cookies.get('googtrans');
    if (websiteLang === '/en/en') {
        Cookies.remove('googtrans');
    }
});

function fragment() {
    window.scrollTo(0, 150);
}

$(window).on('load', function () {
    $('#cover').fadeOut(300);
})



$(function () {
        'use strict';

        class Menu {
            constructor(settings) {
                this.menuNode = settings.menuNode;
                this.state = false;
                this.menuStateTextNode = settings.menuStateTextNode || this.menuNode.querySelector('.menu__screen-reader');
                this.menuOpenedText = settings.menuOpenedText || 'Open menu';
                this.menuClosedText = settings.menuClosedText || 'Close menu';
            }

            changeState(state) {
                return this.state = !state;
            }

            changeStateText(state, node) {
                let text = (state !== true) ? this.menuOpenedText : this.menuClosedText;

                node.textContent = text;
                return text;
            }

            toggleMenuState(className) {

                let state;

                if (typeof className !== 'string' || className.length === 0) {
                    return console.log('you did not give the class for the toggleState function');
                }

                state = this.changeState(this.state);

                this.changeStateText(state, this.menuStateTextNode);
                this.menuNode.classList.toggle(className);

                return state;
            }
        }

        const jsMenuNode = document.querySelector('.menu');
        const demoMenu = new Menu({
            menuNode: jsMenuNode
        });

        function callMenuToggle(event) {
            demoMenu.toggleMenuState('menu_activated');
        }

        jsMenuNode.querySelector('.menu__toggle').addEventListener('click', callMenuToggle);
    })();