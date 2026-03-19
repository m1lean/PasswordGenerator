// Sync checkbox visual state on page load (after form re-render)
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.cb input, .cb-full input').forEach(cb => {
        // set initial visual state
        cb.closest('.cb, .cb-full')?.classList.toggle('on', cb.checked);
        // listen for changes
        cb.addEventListener('change', () => {
            cb.closest('.cb, .cb-full')?.classList.toggle('on', cb.checked);
        });
    });
});
