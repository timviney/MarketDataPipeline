document.addEventListener('DOMContentLoaded', async function () {
    let previousPrice = null;

    const timeMapping = {};

    const chart = Highcharts.chart('container', {
        chart: {
            backgroundColor: 'transparent',
            style: { fontFamily: '-apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif' }
        },
        title: {
            text: 'BARC Closing Price',
            style: { color: '#fff', fontSize: '20px', fontWeight: '600' }
        },
        xAxis: {
            type: 'linear',
            gridLineColor: 'rgba(255, 255, 255, 0.05)',
            lineColor: 'rgba(255, 255, 255, 0.1)',
            tickColor: 'rgba(255, 255, 255, 0.1)',
            labels: {
                rotation: -45,
                style: { color: 'rgba(255, 255, 255, 0.6)' },
                formatter: function () {
                    const realTime = timeMapping[this.value];
                    console.log('Formatting xAxis label for value:', this.value, 'mapped to realTime:', realTime);
                    return realTime
                        ? Highcharts.dateFormat('%d-%m %H:%M', realTime)
                        : '';
                }
            }
        },
        yAxis: {
            title: {
                text: 'Price (GBp)',
                style: { color: 'rgba(255, 255, 255, 0.6)' }
            },
            gridLineColor: 'rgba(255, 255, 255, 0.05)',
            labels: { style: { color: 'rgba(255, 255, 255, 0.6)' } }
        },
        legend: {
            itemStyle: { color: 'rgba(255, 255, 255, 0.8)' },
            itemHoverStyle: { color: '#fff' }
        },
        tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            style: { color: '#fff' },
            borderColor: 'rgba(102, 126, 234, 0.5)',
            borderRadius: 8,
        },
        plotOptions: {
            series: {
                animation: { duration: 800 }
            }
        },
        series: [{
            name: 'Closing Price',
            data: [],
            color: '#667eea',
            lineWidth: 3,
            marker: {
                enabled: false,
                states: {
                    hover: {
                        enabled: true,
                        radius: 5,
                        lineWidth: 2,
                        lineColor: '#667eea'
                    }
                }
            }
        }],
        credits: { enabled: false }
    });

    // LOGIN

    const getTokenUrl = 'http://localhost:5000/login';

    async function getToken() {
        try {
            const response = await fetch(getTokenUrl);
            if (!response.ok) throw new Error('Failed to fetch token');
            const data = await response.json();
            console.log("Successfully minted and received token.");
            return data.token;

        } catch (error) {
            console.error("Token acquisition error:", error);
            return null;
        }
    }

    const jwtToken = await getToken();

    async function authorizedFetch(url, options = {}) {
        if (!jwtToken) {
            console.error("JWT is missing. Cannot make authenticated request.");
            // Attempt to re-fetch and throw if still null
            if (!await getAuthToken()) {
                throw new Error("Authentication required.");
            }
        }

        const headers = {
            // Crucial step: Adding the Bearer token to the Authorization header
            'Authorization': `Bearer ${jwtToken}`,

            // Ensure you preserve any existing headers, or add Content-Type for POSTs
            'Content-Type': 'application/json',
            ...options.headers
        };

        return fetch(url, {
            ...options,
            headers: headers
        });
    }

    ///

    // REPLAY CONTROLS
    const playBtn = document.getElementById('play-btn');
    const pauseBtn = document.getElementById('pause-btn');
    const stopBtn = document.getElementById('stop-btn');
    const speedUpBtn = document.getElementById('speed-up-btn');
    const speedDownBtn = document.getElementById('speed-down-btn');
    const speedDisplayTens = document.getElementById('speed-tens-digit');
    const speedDisplayOnes = document.getElementById('speed-ones-digit');
    const statusEl = document.getElementById('status');
    const statusDot = document.getElementById('status-dot');

    const statusUrl = 'http://localhost:5000/replay/status';
    const playUrl = 'http://localhost:5000/replay/start';
    const pauseUrl = 'http://localhost:5000/replay/pause';
    const stopUrl = 'http://localhost:5000/replay/stop';
    const adjustSpeedUrl = 'http://localhost:5000/replay/adjustspeed';

    async function initialGetStatus() {
        try {
            const response = await authorizedFetch(statusUrl);
            if (!response.ok) throw new Error('Network response was not ok');
            const state = (await response.json()).state;
            setStatus(state.status);
        } catch (error) {
            console.error('Error fetching status:', error);
        }
    }

    function setStatus(status) {
        statusEl.querySelector('span').textContent = status;
        statusDot.className = status === 'Running' ? 'status-dot running' : status === 'Paused' ? 'status-dot paused' : 'status-dot stopped';
        statusEl.className = status === 'Running' ? 'status running' : status === 'Paused' ? 'status paused' : 'status stopped';
    }

    initialGetStatus();

    playBtn.addEventListener('click', () => {
        try {
            authorizedFetch(playUrl, { method: 'POST' });
        } catch (error) {
            console.error('Error starting replay:', error);
        }
        initialGetStatus();
    });

    pauseBtn.addEventListener('click', () => {
        try {
            authorizedFetch(pauseUrl, { method: 'POST' });
        } catch (error) {
            console.error('Error pausing replay:', error);
        }
        initialGetStatus();
    });

    stopBtn.addEventListener('click', () => {
        try {
            authorizedFetch(stopUrl, { method: 'POST' });
        } catch (error) {
            console.error('Error stopping replay:', error);
        }
        initialGetStatus();
    });

    let speed = 1;

    speedUpBtn.addEventListener('click', () => {
        try {
            speed = Math.min(99, speed + 1);
            authorizedFetch(`${adjustSpeedUrl}?speed=${speed}`, { method: 'POST' });
            speedDisplayTens.className = `fa-solid fa-${Math.floor(speed / 10)}`;
            speedDisplayOnes.className = `fa-solid fa-${speed % 10}`;
        } catch (error) {
            console.error('Error speeding up replay:', error);
        }
    });

    speedDownBtn.addEventListener('click', () => {
        try {
            speed = Math.max(1, speed - 1);
            authorizedFetch(`${adjustSpeedUrl}?speed=${speed}`, { method: 'POST' });
            speedDisplayTens.className = `fa-solid fa-${Math.floor(speed / 10)}`;
            speedDisplayOnes.className = `fa-solid fa-${speed % 10}`;
        } catch (error) {
            console.error('Error slowing down replay:', error);
        }
    });

    //////


    // Test SignalR

    const testNotificationConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/notificationHub", {
            accessTokenFactory: () => jwtToken,
            transport: signalR.HttpTransportType.LongPolling // Websockets causing Docker issues locally
        })
        .withAutomaticReconnect()
        .build();

    testNotificationConnection.on("ReceiveMessage", (user, message) => {
        console.log(`Message from ${user}: ${message}`);
    });

    testNotificationConnection.start()
        .then(() => {
            console.log("SignalR Notification Connected.");
        })
        .catch(err => console.error("Error connecting to SignalR Notification: ", err));

    //////

    // Symbol SignalR

    const symbolConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/symbolHub", {
            accessTokenFactory: () => jwtToken,
            transport: signalR.HttpTransportType.LongPolling // Websockets causing Docker issues locally
        })
        .withAutomaticReconnect()
        .build();

    symbolConnection.on("LatestCalculation", (symbol, calculations) => {
        console.log(`Latest calculation for ${symbol}: ${calculations}`);
        updateData(symbol, calculations);
    });

    symbolConnection.start()
        .then(() => {
            console.log("SignalR Symbols Connected.");
        })
        .catch(err => console.error("Error connecting to SignalR Symbols: ", err));

    //////

    // Replay SignalR

    const replayConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/replayHub", {
            accessTokenFactory: () => jwtToken,
            transport: signalR.HttpTransportType.LongPolling // Websockets causing Docker issues locally
        })
        .withAutomaticReconnect()
        .build();

    replayConnection.on("GetState", (state) => {
        console.log(`Latest state: ${state}`);
        setStatus(state.status);
    });


    replayConnection.on("HasBeenCleared", () => {
        console.log(`Data has been cleared.`);
        fetchChartData();
    });

    replayConnection.start()
        .then(() => {
            console.log("SignalR Replay Connected.");
        })
        .catch(err => console.error("Error connecting to SignalR Replay: ", err));
    //////


    // DATA FETCHING AND CHART UPDATING

    const calculationsUrl = 'http://localhost:5000/symbols/BARC/calculations';

    async function fetchChartData() {
        try {
            const response = await authorizedFetch(calculationsUrl);
            if (!response.ok) throw new Error('Network response was not ok');

            const dictionaryData = await response.json();
            const series = chart.series[0];
            const newData = [];

            for (const dateTimeKey in dictionaryData) {
                if (dictionaryData.hasOwnProperty(dateTimeKey)) {
                    const calculations = dictionaryData[dateTimeKey];
                    const t = new Date(dateTimeKey).getTime();
                    const sma = calculations.dailySma;
                    const closingPrice = calculations.tick.close;

                    newData.push({ y: closingPrice, realTime: t, sma: sma });
                }
            }

            Object.keys(timeMapping).forEach(key => delete timeMapping[key]);

            newData.sort((a, b) => a.realTime - b.realTime);
            let barInx = 0;
            newData.forEach(point => {
                point.x = barInx;
                timeMapping[barInx] = point.realTime;
                barInx++;
            });

            series.setData(newData, true);
            updateStats(newData);

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    fetchChartData(); // Get initial data

    async function updateData(symbol, calculations) {
        try {
            if (symbol !== 'BARC') return; // Only process BARC updates for now

            const series = chart.series[0];

            const t = new Date(calculations.tick.dateTime).getTime();
            const sma = calculations.dailySma;
            const closingPrice = calculations.tick.close;

            const newX = series.data.length;
            timeMapping[newX] = t;

            series.addPoint({ x: newX, y: closingPrice, realTime: t, sma: sma }, true);

            chart.xAxis[0].update({}, true);

            updateStats(series.data);

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    function updateStats(newData) {
        if (newData.length > 0) {
            const latestPoint = newData[newData.length - 1];
            const currentPrice = latestPoint.y;

            document.getElementById('current-price').textContent = currentPrice.toFixed(2) + 'p';
            document.getElementById('daily-sma').textContent = latestPoint.sma.toFixed(2) + 'p';
            document.getElementById('data-points').textContent = newData.length;
            document.getElementById('last-update').textContent = new Date().toLocaleTimeString();

            if (previousPrice !== null) {
                const change = currentPrice - previousPrice;
                const changePercent = ((change / previousPrice) * 100).toFixed(2);
                const changeEl = document.getElementById('price-change');
                changeEl.textContent = `${change >= 0 ? '↑' : '↓'} ${Math.abs(change).toFixed(2)}p (${changePercent}%)`;
                changeEl.className = 'stat-change ' + (change >= 0 ? 'positive' : 'negative');
            }

            previousPrice = currentPrice;
        }
    }

    //////
});
