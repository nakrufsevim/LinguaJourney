document.addEventListener("DOMContentLoaded", () => {
    document.body.classList.add("is-ready");

    const flashMessage = document.querySelector(".flash-message");
    if (flashMessage) {
        window.setTimeout(() => {
            flashMessage.classList.add("flash-message--fade");
        }, 4200);
    }
});
