function createToast(text, colorClass = 'is-primary') {
    const id = '_' + Math.random().toString(36).substr(2, 9);
    const html = `<div class="notification ${colorClass} has-fadein" id="${id}"><button class="delete"></button>${text}</div>`;
    $(".toast-container").append(html);
    const element = $(`#${id}`);
    setTimeout(function () {
        element.fadeOut();
        element.remove();
    }, 7000);
};

function openModalByEvent(event) {
    event.stopPropagation();
    event.preventDefault();
    const id = event.currentTarget.dataset.target;
    const modal = $(id);
    modal.toggleClass('is-active');
    $('html').toggleClass('is-clipped');
    const openEvent = jQuery.Event("modal:open");
    openEvent.relatedTarget = event.currentTarget;
    modal.trigger(openEvent);
};

function closeModalByEvent(event) {
    event.stopPropagation();
    const modal = $(this).closest(".modal");
    modal.toggleClass('is-active');
    $('html').toggleClass('is-clipped');
    modal.trigger("modal:close");
}

function handleModal(args) {
    const defaults = {
        id: '',
        token: '',
        init: {
            datainfo: '',
            action: function (target, data) { }
        },
        load: {
            dataurl: '',
            action: function (target, data) { },
            toast: { failed: 'Fehler' }
        },
        confirm: {
            dataurl: '',
            pre: function (target, url) { return url; },
            post: function (data) { return true; },
            toast: { success: 'OK', failed: 'Fehler' }
        },
    };
    const params = { ...defaults, ...args };
    const modal = $(params.id);
    modal.on('modal:open', function (e) {
        if (params.init.datainfo) {
            const info = e.relatedTarget.dataset[params.init.datainfo];
            params.init.action(e.target, info);
        }
        const confirm = $(e.target).find(".confirm");
        const loading = $(e.target).find('.loading-value');

        if (params.load.dataurl) {
            loading.removeClass('is-hidden');
            const url = e.relatedTarget.dataset[params.load.dataurl];
            $.post(url, params.token).done(function (data) {
                if (data) {
                    loading.addClass('is-hidden');
                    params.load.action(e.target, data);
                    if (confirm) {
                        confirm.attr("disabled", false);
                    }
                } else {
                    createToast(params.load.toast.failed, 'is-danger');
                }
            });
        } else {
            if (confirm) {
                confirm.attr("disabled", false);
            }
        }

        if (params.confirm.dataurl) {
            const dataurl = e.relatedTarget.dataset[params.confirm.dataurl];
            
            confirm.click(function (evClick) {
                evClick.preventDefault();
                confirm.addClass("is-loading");
                const url = params.confirm.pre ? params.confirm.pre(e.target, dataurl) : dataurl;
                $.post(url, params.token).done(function (data) {
                    if (data) {
                        const showToast = params.confirm.post(data);
                        if (showToast) {
                            createToast(params.confirm.toast.success);
                        }
                    } else {
                        createToast(params.confirm.toast.failed, 'is-danger');
                    }
                });
            });
        }
    });
    modal.on('modal:close', function (e) {
        $(e.target).find('.loading-value').addClass('is-hidden');
        if (params.init.datainfo) {
            $(e.target).find(params.init.selector).text('');
        }
        if (params.load.dataurl) {
            $(e.target).find(params.load.selector).text('');
        }
        const confirm = $(e.target).find(".confirm");
        if (confirm) {
            confirm.attr("disabled", true);
            confirm.removeClass("is-loading");
            confirm.off();
        }
    });
}

function sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
}

function localStorageSetItem(key, value, ttl) {
    const item = {
        value: value,
        expiration: new Date().getTime() + ttl
    };
    localStorage.setItem(key, JSON.stringify(item));
}

function localStorageGetItem(key) {
    const itemStr = localStorage.getItem(key);
    if (!itemStr) {
        return null;
    }
    const item = JSON.parse(itemStr);
    if (new Date().getTime() > item.expiration) {
        localStorage.removeItem(key);
        return null;
    }
    return item.value;
}

const trumbowygConfig = {
    resetCss: true,
    lang: 'de',
    semantic: { 'del': 's' },
    autogrowOnEnter: true,
    tagsToRemove: ['script', 'style', 'link', 'iframe', 'input', 'embed', 'img', 'table'],
    btns: [
        ['undo', 'redo'],
        ['formatting'],
        ['strong', 'em', 'del'],
        ['foreColor', 'backColor'],
        ['justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull'],
        ['emoji'],
        ['unorderedList', 'orderedList'],
        ['horizontalRule'],
        ['removeformat'],
        ['fullscreen']
    ]
};

$(function () {
    $(".navbar-burger").click(function () {
        $(".navbar-burger").toggleClass("is-active");
        $(".navbar-menu").toggleClass("is-active");
    });

    // should work for dynamic created elements also
    $("body").on("click", ".notification > button.delete", function () {
        $(this).parent().addClass('is-hidden').remove();
        return false;
    });

    $('.open-modal').click(openModalByEvent);
    $('.close-modal').click(closeModalByEvent);

    $('.is-toggle-password').click(function () {
        const input = $(this).parent().parent().find('input');
        const isPassword = input.attr('type') === 'password';
        input.attr('type', isPassword ? 'text' : 'password');
    });

    $(".clickable-row").click(function (e) {
        e.stopPropagation();
        window.location = $(this).data("url");
    });

    $(".clickable-row-open").click(function (e) {
        e.stopPropagation();
        window.open($(this).data("url"), '_blank', 'noopener,noreferrer');
        return true;
    });
});