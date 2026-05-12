document.addEventListener("DOMContentLoaded", () => {
    document.body.classList.add("is-ready");

    const flashMessage = document.querySelector(".flash-message");
    if (flashMessage) {
        window.setTimeout(() => {
            flashMessage.classList.add("flash-message--fade");
        }, 4200);
    }

    const scheduledDateInput = document.getElementById("ScheduledDate");
    const completedInput = document.getElementById("IsCompleted");
    if (scheduledDateInput && completedInput) {
        const defaultMin = scheduledDateInput.dataset.defaultMin;
        const syncLessonDateRule = () => {
            if (completedInput.checked) {
                scheduledDateInput.removeAttribute("min");
                return;
            }

            if (defaultMin) {
                scheduledDateInput.setAttribute("min", defaultMin);
            }
        };

        completedInput.addEventListener("change", syncLessonDateRule);
        syncLessonDateRule();
    }

    const practicedOnInput = document.getElementById("PracticedOn");
    const nextReviewDateInput = document.getElementById("NextReviewDate");
    if (practicedOnInput && nextReviewDateInput) {
        const today = nextReviewDateInput.dataset.today;
        const isCreateMode = nextReviewDateInput.dataset.createMode === "true";
        const syncPracticeDateRule = () => {
            const minimumDate = isCreateMode
                ? today
                : (practicedOnInput.value || today);

            if (minimumDate) {
                nextReviewDateInput.setAttribute("min", minimumDate);
            }
        };

        practicedOnInput.addEventListener("change", syncPracticeDateRule);
        syncPracticeDateRule();
    }
});
