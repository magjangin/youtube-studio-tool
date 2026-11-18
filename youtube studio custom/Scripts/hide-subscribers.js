// Hides subscriber counts (static metrics + the realtime widget) on YouTube Studio.
// Registered as a WebView2 document-created script, so this runs before <html>/<head> exist yet -
// document.head and document.documentElement are both null at that point. The MutationObserver
// below is what actually lands the style, as soon as the parser adds a node to attach it to; it's
// also what keeps it alive across YouTube Studio's SPA client-side route changes.
(function () {
    function injectStyle() {
        if (document.getElementById('hide-sub-style')) return;
        const target = document.head || document.documentElement;
        if (!target) return;

        const style = document.createElement('style');
        style.id = 'hide-sub-style';
        style.textContent = `
            .metric-value-big,
            .subscribers-count,
            #subscribers-count,
            ytcp-stat-metric[metric-id="SUBSCRIBERS"] .value,
            ytcp-animating-number,
            #count,
            #count-container,
            #primary-value,
            ytcp-sub-count-dialog #count-container,
            .metric-value.yta-latest-activity-card,
            yt-card-view-model[data-hide-sub-card="true"] ytcp-simple-metric-view-model,
            yt-card-view-model[data-hide-sub-card="true"] .ytcpSimpleMetricValue {
                visibility: hidden !important;
                color: transparent !important;
            }

            /* Advanced mode > Realtime performance (explore_type=LATEST_ACTIVITY): the pinned
               total row's mini bar charts sit right above the per-video ones and read like an
               extra video row. Hide those charts, plus the last-60-minutes total next to them;
               the 48-hour total (#debug-hourly-total) stays. */
            yta-explore-latest-activity-row #row-container[total-row] yta-tiny-chart,
            yta-explore-latest-activity-row #row-container[total-row] #debug-minutely-total {
                visibility: hidden !important;
            }
        `;
        target.appendChild(style);
    }

    const TARGET_CARD_TITLES = new Set(['총 구독자 수', 'Total subscribers']);

    function updateSubscriberCards() {
        const titles = document.querySelectorAll('yt-card-view-model .ytShelfHeaderLayoutTitle');
        for (const titleEl of titles) {
            const card = titleEl.closest('yt-card-view-model');
            if (!card) continue;

            if (TARGET_CARD_TITLES.has(titleEl.textContent.trim())) {
                if (card.getAttribute('data-hide-sub-card') !== 'true') {
                    card.setAttribute('data-hide-sub-card', 'true');
                }
            } else if (card.hasAttribute('data-hide-sub-card')) {
                card.removeAttribute('data-hide-sub-card');
            }
        }
    }

    function applyHiding() {
        injectStyle();
        updateSubscriberCards();
    }

    applyHiding();

    if (window.__hideSubCountObserver) {
        window.__hideSubCountObserver.disconnect();
    }
    const observer = new MutationObserver(applyHiding);
    observer.observe(document, { childList: true, subtree: true, characterData: true });
    window.__hideSubCountObserver = observer;
})();
