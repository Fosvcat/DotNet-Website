// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    initCommentVoting();
    initReplyToggles();
    initNotificationReadButtons();
    initHeroTypewriter();
    initThemeAmbientBackground();
    initSecurityKnowledgeCheck();
});

function initSecurityKnowledgeCheck() {
    var quiz = document.querySelector('[data-security-quiz]');
    if (!quiz) {
        return;
    }

    var submitButton = quiz.querySelector('[data-quiz-submit]');
    var result = quiz.querySelector('[data-quiz-result]');
    var questions = Array.from(quiz.querySelectorAll('.python-question'));

    submitButton.addEventListener('click', function () {
        var correctCount = 0;
        var answeredCount = 0;

        questions.forEach(function (question) {
            var selected = question.querySelector('input[type="radio"]:checked');
            var feedback = question.querySelector('.python-question-feedback');
            var isCorrect = selected && selected.value === question.dataset.correctAnswer;

            question.classList.remove('is-correct', 'is-incorrect', 'is-unanswered');

            if (!selected) {
                question.classList.add('is-unanswered');
                feedback.textContent = 'Select an answer before checking.';
                return;
            }

            answeredCount += 1;
            if (isCorrect) {
                correctCount += 1;
                question.classList.add('is-correct');
                feedback.textContent = 'Correct — this matches the strongest practice described above.';
            } else {
                question.classList.add('is-incorrect');
                feedback.textContent = 'Not quite. Review the operating principle above and try again.';
            }
        });

        if (answeredCount < questions.length) {
            result.textContent = 'Complete all three questions to receive a final score.';
            return;
        }

        result.textContent = correctCount === questions.length
            ? '3 / 3 — Knowledge check complete.'
            : correctCount + ' / ' + questions.length + ' correct — adjust your selections and check again.';
    });
}

// Theme-specific background interaction for the main browsing pages.
// Light: a low-contrast technical grid plus a short terminal-glyph/particle
// burst when empty background is clicked.
// Dark: a continuously moving dotted particle ribbon. Every empty-background
// click creates one short particle-wave segment in the next accent color;
// the segment moves, disperses, and fades away on its own.
function initThemeAmbientBackground() {
    var canvas = document.getElementById('themeAmbientCanvas');
    if (!canvas) {
        return;
    }

    var context = canvas.getContext('2d');
    if (!context) {
        return;
    }

    var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
    var width = 0;
    var height = 0;
    var pixelRatio = 1;
    var terminalSymbols = [];
    var terminalParticles = [];
    var particleWaveSegments = [];
    var nextParticleWaveColor = 0;
    var animationFrame = 0;
    var lastAnimationTimestamp = 0;
    var targetFrameInterval = 1000 / 30;
    var cachedParticleWaveColors = [];
    var glyphs = [
        '>_', '$', './', '$_', '~/', '#!', '0x', '::',
        '{}', '[]', '</>', '&&', '|>', '/*', '*/', '=>'
    ];
    var symbolSizes = [18, 24, 34, 46, 60, 74];

    function currentTheme() {
        return document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
    }

    function themeColor(variableName, fallback) {
        var value = getComputedStyle(document.documentElement).getPropertyValue(variableName).trim();
        return value || fallback;
    }

    function resizeCanvas() {
        width = window.innerWidth;
        height = window.innerHeight;
        // A decorative full-screen canvas does not need retina-level backing
        // resolution. Capping it keeps the fill-rate predictable on 2K/4K
        // and high-DPI displays without making the particles look soft.
        pixelRatio = Math.min(window.devicePixelRatio || 1, 1.25);
        canvas.width = Math.round(width * pixelRatio);
        canvas.height = Math.round(height * pixelRatio);
        canvas.style.width = width + 'px';
        canvas.style.height = height + 'px';
        context.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
        drawStillFrame();
    }

    function isBlankBackground(target) {
        if (!(target instanceof Element)) {
            return true;
        }

        return !target.closest(
            'a, button, input, textarea, select, option, label, form, nav, header, footer, ' +
            '.page-banner, .table-card, .card, .dropdown-menu, .modal, .alert, .list-group, ' +
            'article, .comment-card, [role="dialog"], [contenteditable="true"]'
        );
    }

    function addTerminalSymbol(x, y, now) {
        var colorIndex = Math.floor(Math.random() * 6);
        var duration = 760 + Math.random() * 240;
        var baseSize = symbolSizes[Math.floor(Math.random() * symbolSizes.length)];

        terminalSymbols.push({
            x: x,
            y: y,
            glyph: glyphs[Math.floor(Math.random() * glyphs.length)],
            bornAt: now,
            duration: duration,
            colorIndex: colorIndex,
            fontSize: baseSize + (Math.random() - 0.5) * Math.max(4, baseSize * 0.16),
            rotation: (Math.random() - 0.5) * 0.16
        });

        var particleCount = 8 + Math.floor(Math.random() * 7);
        for (var particleIndex = 0; particleIndex < particleCount; particleIndex++) {
            var angle = Math.random() * Math.PI * 2;
            var speed = 34 + Math.random() * 82;
            terminalParticles.push({
                x: x,
                y: y,
                bornAt: now,
                duration: 480 + Math.random() * 360,
                colorIndex: Math.random() < 0.55 ? colorIndex : Math.floor(Math.random() * 6),
                radius: 0.8 + Math.random() * 1.8,
                velocityX: Math.cos(angle) * speed,
                velocityY: Math.sin(angle) * speed - 14
            });
        }

        if (terminalSymbols.length > 10) {
            terminalSymbols.shift();
        }
        if (terminalParticles.length > 120) {
            terminalParticles.splice(0, terminalParticles.length - 120);
        }
    }

    function addWaveInteraction(x, now) {
        particleWaveSegments.push({
            x: x,
            y: Math.max(120, Math.min(height - 110, lastPointerY)),
            bornAt: now,
            duration: 1700 + Math.random() * 500,
            colorIndex: nextParticleWaveColor,
            halfWidth: Math.min(210 + Math.random() * 80, width * 0.34),
            amplitude: 46 + Math.random() * 30,
            frequency: 0.029 + Math.random() * 0.012,
            phase: Math.random() * Math.PI * 2,
            direction: Math.random() < 0.5 ? -1 : 1
        });
        nextParticleWaveColor = (nextParticleWaveColor + 1) % 6;

        if (particleWaveSegments.length > 6) {
            particleWaveSegments.shift();
        }
    }

    var lastPointerY = height * 0.5;

    document.addEventListener('pointerdown', function (event) {
        if (event.button !== 0 || !isBlankBackground(event.target)) {
            return;
        }

        var now = performance.now();
        if (currentTheme() === 'light') {
            addTerminalSymbol(event.clientX, event.clientY, now);
        } else {
            lastPointerY = event.clientY;
            addWaveInteraction(event.clientX, now);
        }

        if (reduceMotion.matches) {
            drawStillFrame();
            window.setTimeout(drawStillFrame, 1200);
        }
    });

    function smoothStep(value) {
        var clamped = Math.max(0, Math.min(1, value));
        return clamped * clamped * (3 - 2 * clamped);
    }

    function drawTechnologyGrid() {
        var gridColor = themeColor('--gs-ambient-1', '#168fc8');
        var spacing = 34;
        var majorEvery = 5;

        context.save();
        context.lineWidth = 1;

        for (var x = 0, column = 0; x <= width; x += spacing, column++) {
            context.beginPath();
            context.strokeStyle = gridColor;
            context.globalAlpha = column % majorEvery === 0 ? 0.075 : 0.035;
            context.moveTo(Math.round(x) + 0.5, 0);
            context.lineTo(Math.round(x) + 0.5, height);
            context.stroke();
        }

        for (var y = 0, row = 0; y <= height; y += spacing, row++) {
            context.beginPath();
            context.strokeStyle = gridColor;
            context.globalAlpha = row % majorEvery === 0 ? 0.075 : 0.035;
            context.moveTo(0, Math.round(y) + 0.5);
            context.lineTo(width, Math.round(y) + 0.5);
            context.stroke();
        }

        context.fillStyle = gridColor;
        context.globalAlpha = 0.12;
        for (var dotX = 0; dotX <= width; dotX += spacing * majorEvery) {
            for (var dotY = 0; dotY <= height; dotY += spacing * majorEvery) {
                context.beginPath();
                context.arc(dotX, dotY, 1.15, 0, Math.PI * 2);
                context.fill();
            }
        }
        context.restore();
    }

    function drawTerminalSymbols(now) {
        var colors = [
            themeColor('--gs-ambient-1', '#168fc8'),
            themeColor('--gs-ambient-2', '#356bd8'),
            themeColor('--gs-ambient-3', '#6857c8'),
            themeColor('--gs-ambient-4', '#f2c94c'),
            themeColor('--gs-ambient-5', '#ef476f'),
            themeColor('--gs-ambient-6', '#172033')
        ];

        terminalSymbols = terminalSymbols.filter(function (symbol) {
            return now - symbol.bornAt < symbol.duration;
        });
        terminalParticles = terminalParticles.filter(function (particle) {
            return now - particle.bornAt < particle.duration;
        });

        terminalSymbols.forEach(function (symbol) {
            var age = now - symbol.bornAt;
            var progress = Math.min(age / symbol.duration, 1);
            var jellyProgress = Math.min(progress / 0.48, 1);
            var scale = 1 - Math.exp(-7.5 * jellyProgress) * Math.cos(21 * jellyProgress);
            var fade = 1 - smoothStep((progress - 0.36) / 0.64);
            var bounceY = -7 * Math.sin(Math.min(progress / 0.42, 1) * Math.PI);

            context.save();
            context.translate(symbol.x, symbol.y + bounceY);
            context.rotate(symbol.rotation);
            context.scale(scale, scale);
            context.globalAlpha = fade * 0.58;
            context.fillStyle = colors[symbol.colorIndex];
            context.shadowColor = colors[symbol.colorIndex];
            context.shadowBlur = 7;
            context.font = '700 ' + symbol.fontSize + 'px ui-monospace, SFMono-Regular, Consolas, monospace';
            context.textAlign = 'center';
            context.textBaseline = 'middle';
            context.fillText(symbol.glyph, 0, 0);
            context.restore();
        });

        terminalParticles.forEach(function (particle) {
            var age = now - particle.bornAt;
            var progress = Math.min(age / particle.duration, 1);
            var elapsedSeconds = age / 1000;
            var particleX = particle.x + particle.velocityX * elapsedSeconds;
            var particleY = particle.y + particle.velocityY * elapsedSeconds + 58 * elapsedSeconds * elapsedSeconds;

            context.save();
            context.globalAlpha = (1 - smoothStep(progress)) * 0.52;
            context.fillStyle = colors[particle.colorIndex];
            context.shadowColor = colors[particle.colorIndex];
            context.shadowBlur = 4;
            context.beginPath();
            context.arc(particleX, particleY, particle.radius * (1 - progress * 0.45), 0, Math.PI * 2);
            context.fill();
            context.restore();
        });
    }

    function particleWaveColors() {
        if (!cachedParticleWaveColors.length) {
            cachedParticleWaveColors = [
                themeColor('--gs-ambient-1', '#42c9e8'),
                themeColor('--gs-ambient-2', '#6f82ff'),
                themeColor('--gs-ambient-3', '#ff6f91'),
                themeColor('--gs-ambient-4', '#ffd166'),
                themeColor('--gs-ambient-5', '#ff647c'),
                themeColor('--gs-ambient-6', '#d7e0f2')
            ];
        }

        return cachedParticleWaveColors;
    }

    function drawParticleDot(x, y, radius, alpha) {
        context.globalAlpha = alpha;
        context.beginPath();
        context.arc(x, y, radius, 0, Math.PI * 2);
        context.fill();
    }

    function drawAmbientParticleWave(now) {
        var colors = particleWaveColors();
        var baseY = height * 0.5;
        var layerCount = 8;
        var spacing = width >= 1700 ? 28 : 24;

        context.save();
        context.globalCompositeOperation = 'lighter';
        context.shadowBlur = 0;

        for (var layer = 0; layer < layerCount; layer++) {
            var depth = (layer - (layerCount - 1) / 2) / ((layerCount - 1) / 2);
            var depthPhase = depth * 1.34;
            var color = colors[layer % 3];
            context.fillStyle = color;

            for (var x = -spacing; x <= width + spacing; x += spacing) {
                var phase = x * 0.0084 - now * 0.00082;
                var particleX = x + Math.sin(phase * 0.72 + depthPhase) * Math.abs(depth) * 20;
                var particleY = baseY +
                    Math.sin(phase + depthPhase) * 58 +
                    Math.cos(phase * 0.58 - depthPhase) * depth * 52;
                var frontness = 0.55 + 0.45 * Math.cos(phase + depthPhase);
                var radius = 1.25 + frontness * 1.6;
                var alpha = 0.025 + frontness * 0.075;

                drawParticleDot(particleX, particleY, radius, alpha);
            }
        }

        context.restore();
    }

    function drawClickedParticleWaves(now) {
        var colors = particleWaveColors();

        particleWaveSegments = particleWaveSegments.filter(function (segment) {
            return now - segment.bornAt < segment.duration;
        });

        particleWaveSegments.forEach(function (segment) {
            var age = now - segment.bornAt;
            var progress = Math.min(age / segment.duration, 1);
            var appear = smoothStep(Math.min(progress / 0.12, 1));
            var disappear = 1 - smoothStep(Math.max(0, (progress - 0.24) / 0.76));
            var opacity = appear * disappear;
            var color = colors[segment.colorIndex];
            var layerCount = 7;
            var spacing = 16;
            var drift = segment.direction * progress * 34;

            context.save();
            context.globalCompositeOperation = 'lighter';
            context.fillStyle = color;
            context.shadowBlur = 0;

            for (var layer = 0; layer < layerCount; layer++) {
                var depth = (layer - (layerCount - 1) / 2) / ((layerCount - 1) / 2);
                var depthPhase = depth * 1.42;

                for (var localX = -segment.halfWidth; localX <= segment.halfWidth; localX += spacing) {
                    var edgeProgress = Math.abs(localX) / segment.halfWidth;
                    var edgeFade = Math.pow(Math.max(0, 1 - edgeProgress), 0.65);
                    var travellingPhase =
                        localX * segment.frequency -
                        now * 0.0042 * segment.direction +
                        segment.phase;
                    var particleX = segment.x + localX + drift +
                        Math.sin(travellingPhase * 0.68 + depthPhase) * Math.abs(depth) * 14;
                    var particleY = segment.y +
                        Math.sin(travellingPhase + depthPhase) * segment.amplitude * edgeFade +
                        Math.cos(travellingPhase * 0.56 - depthPhase) * depth * 38 * edgeFade;
                    var frontness = 0.55 + 0.45 * Math.cos(travellingPhase + depthPhase);
                    var radius = (1.35 + frontness * 2.05) * (0.78 + edgeFade * 0.22);
                    var alpha = opacity * edgeFade * (0.16 + frontness * 0.4);

                    drawParticleDot(particleX, particleY, radius, alpha);
                }
            }

            context.restore();
        });
    }

    function drawParticleWaves(now) {
        drawAmbientParticleWave(now);
        drawClickedParticleWaves(now);
    }

    function drawStillFrame() {
        context.clearRect(0, 0, width, height);
        var now = performance.now();
        if (currentTheme() === 'light') {
            drawTechnologyGrid();
            drawTerminalSymbols(now);
        } else {
            drawParticleWaves(now);
        }
    }

    function drawAnimatedFrame(now) {
        animationFrame = window.requestAnimationFrame(drawAnimatedFrame);

        if (now - lastAnimationTimestamp < targetFrameInterval) {
            return;
        }
        lastAnimationTimestamp = now - ((now - lastAnimationTimestamp) % targetFrameInterval);

        context.clearRect(0, 0, width, height);
        if (currentTheme() === 'light') {
            drawTechnologyGrid();
            drawTerminalSymbols(now);
        } else {
            drawParticleWaves(now);
        }
    }

    var themeObserver = new MutationObserver(function () {
        terminalSymbols = [];
        terminalParticles = [];
        particleWaveSegments = [];
        nextParticleWaveColor = 0;
        cachedParticleWaveColors = [];
        lastAnimationTimestamp = 0;
        drawStillFrame();
    });

    themeObserver.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ['data-bs-theme']
    });

    window.addEventListener('resize', resizeCanvas);
    reduceMotion.addEventListener('change', function () {
        if (animationFrame) {
            window.cancelAnimationFrame(animationFrame);
            animationFrame = 0;
        }

        if (reduceMotion.matches) {
            drawStillFrame();
        } else {
            animationFrame = window.requestAnimationFrame(drawAnimatedFrame);
        }
    });

    resizeCanvas();
    if (!reduceMotion.matches) {
        animationFrame = window.requestAnimationFrame(drawAnimatedFrame);
    }
}

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
                credentials: 'same-origin'
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

// Marks a single notification as read via fetch(), with no page
// reload: the unread dot and the Read button itself just disappear.
function initNotificationReadButtons() {
    var tokenInput = document.querySelector('#antiForgeryTokenForm input[name="__RequestVerificationToken"]');
    if (!tokenInput) {
        return;
    }
    var token = tokenInput.value;

    document.querySelectorAll('.mark-read-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = btn.getAttribute('data-notification-id');

            fetch('/Notification/MarkRead', {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'id=' + encodeURIComponent(id) + '&__RequestVerificationToken=' + encodeURIComponent(token)
            })
                .then(function (response) {
                    if (!response.ok) {
                        return;
                    }
                    var row = document.getElementById('notification-row-' + id);
                    if (row) {
                        var dot = row.querySelector('.unread-dot');
                        if (dot) {
                            dot.remove();
                        }
                    }
                    btn.remove();
                })
                .catch(function (error) {
                    console.error('Mark-as-read failed:', error);
                });
        });
    });
}

// Homepage hero typewriter effect: types out each phrase, pauses with a
// blinking cursor, deletes it, then moves on to the next — looping
// forever. Starts immediately on page load. Only runs on the homepage
// (the target element only exists there).
function initHeroTypewriter() {
    var target = document.getElementById('typewriter-text');
    if (!target) {
        return;
    }

    var phrases = [
        'the web a safer place',
        'cyber threats obsolete',
        'you the ultimate defender'
    ];

    var phraseIndex = 0;
    var charIndex = 0;
    var typingSpeed = 55;
    var deletingSpeed = 30;
    var pauseAfterTyping = 1800;
    var pauseAfterDeleting = 300;

    function type() {
        var current = phrases[phraseIndex];
        if (charIndex < current.length) {
            target.textContent = current.substring(0, charIndex + 1);
            charIndex++;
            setTimeout(type, typingSpeed);
        } else {
            setTimeout(erase, pauseAfterTyping);
        }
    }

    function erase() {
        var current = phrases[phraseIndex];
        if (charIndex > 0) {
            charIndex--;
            target.textContent = current.substring(0, charIndex);
            setTimeout(erase, deletingSpeed);
        } else {
            phraseIndex = (phraseIndex + 1) % phrases.length;
            setTimeout(type, pauseAfterDeleting);
        }
    }

    type();
}
