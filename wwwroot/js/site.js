// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    initCommentVoting();
    initReplyToggles();
});

// Intercepts the like/dislike form submissions and sends them via
// fetch() instead of a normal browser form submission, so voting never
// triggers a full page navigation/reload. The button state and count
// are updated directly from the JSON response.
function initCommentVoting() {
    document.querySelectorAll('.vote-form').forEach(function (form) {
        form.addEventListener('submit', function (event) {
            event.preventDefault();

            var formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(function (response) {
                    var contentType = response.headers.get('content-type') || '';
                    if (!contentType.includes('application/json')) {
                        // Not authenticated — the [Authorize] challenge
                        // redirected to the login page instead of running
                        // the action. Follow it like a normal navigation.
                        window.location.href = response.url;
                        return null;
                    }
                    return response.json();
                })
                .then(function (data) {
                    if (!data) {
                        return;
                    }

                    var card = form.closest('.comment-card');
                    if (!card) {
                        return;
                    }

                    var likeBtn = card.querySelector('.like-btn');
                    var dislikeBtn = card.querySelector('.dislike-btn');
                    var likeCountEl = card.querySelector('.like-count');
                    var dislikeCountEl = card.querySelector('.dislike-count');

                    if (likeCountEl) likeCountEl.textContent = data.likeCount;
                    if (dislikeCountEl) dislikeCountEl.textContent = data.dislikeCount;
                    if (likeBtn) likeBtn.classList.toggle('voted-like', data.myVote === true);
                    if (dislikeBtn) dislikeBtn.classList.toggle('voted-dislike', data.myVote === false);
                })
                .catch(function (error) {
                    console.error('Vote request failed:', error);
                });
        });
    });
}

// Toggles the small inline reply form below a comment when its
// "Reply" link is clicked, instead of navigating anywhere.
function initReplyToggles() {
    document.querySelectorAll('.reply-toggle').forEach(function (link) {
        link.addEventListener('click', function (event) {
            event.preventDefault();

            var id = link.getAttribute('data-comment-id');
            var container = document.getElementById('reply-form-' + id);
            if (!container) {
                return;
            }

            var isHidden = container.style.display === 'none' || container.style.display === '';
            container.style.display = isHidden ? 'block' : 'none';

            if (isHidden) {
                var textarea = container.querySelector('textarea');
                if (textarea) {
                    textarea.focus();
                }
            }
        });
    });
}
