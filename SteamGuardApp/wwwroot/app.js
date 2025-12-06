function copyToClipboard(text) {
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(function () {
            console.log('Text copied to clipboard');
        }).catch(function (err) {
            console.error('Error in copying text: ', err);
        });
    } else {
        // Fallback for unsupported clipboard API
        var textArea = document.createElement("textarea");
        textArea.value = text;
        textArea.style.position = "fixed";  // Avoid scrolling to bottom
        textArea.style.top = 0;
        textArea.style.left = 0;
        textArea.style.width = '2em';
        textArea.style.height = '2em';
        textArea.style.padding = 0;
        textArea.style.border = 'none';
        textArea.style.outline = 'none';
        textArea.style.boxShadow = 'none';
        textArea.style.background = 'transparent';
        document.body.appendChild(textArea);
        textArea.select();
        try {
            var successful = document.execCommand('copy');
            console.log('Fallback: Copying text command was ' + (successful ? 'successful' : 'unsuccessful'));
        } catch (err) {
            console.error('Fallback: Oops, unable to copy', err);
        }
        document.body.removeChild(textArea);
    }
}

function initializeTooltips() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
}

function updateTooltipContent(element, newContent) {
    try {
        const tooltip = bootstrap.Tooltip.getInstance(element);
        tooltip.setContent({ '.tooltip-inner': newContent });
    } catch (err) {
        console.error('Error updating tooltip content: ', err);
    }
}

function initScrollingText() {
    const elems = document.querySelectorAll(".scrolling-text");

    elems.forEach(el => {
        if (el.dataset.init === "1") return;
        el.dataset.init = "1";

        const parent = el.parentElement;
        if (!parent) return;

        // 必要样式（不用 CSS 文件）
        el.style.whiteSpace = "nowrap";
        el.style.position = "relative";
        el.style.transform = "translateX(0)";
        el.style.transition = "none";

        const speed = 3;            // 每秒 3px
        const pause = 1000;           // 边缘停顿 1 秒
        const frame = 1000 / 60;     // 60 FPS

        function startScrolling() {
            const parentWidth = parent.clientWidth;
            const textWidth = el.scrollWidth;

            if (textWidth <= parentWidth) return;

            const minX = parentWidth - textWidth;
            const maxX = 0;

            let pos = 0;
            let dir = -1;

            function animate() {
                pos += dir * (speed * frame / 1000);
                el.style.transform = `translateX(${pos}px)`;

                if (pos <= minX) {
                    dir = 1;
                    setTimeout(() => requestAnimationFrame(animate), pause);
                    return;
                }

                if (pos >= maxX) {
                    dir = -1;
                    setTimeout(() => requestAnimationFrame(animate), pause);
                    return;
                }

                requestAnimationFrame(animate);
            }

            requestAnimationFrame(animate);
        }

        startScrolling();
    });
};
