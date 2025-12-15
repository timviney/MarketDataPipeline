document.addEventListener('DOMContentLoaded', async function () {
    let previousPrice = null;

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
                style: { color: 'rgba(255, 255, 255, 0.6)' },
                formatter: function () {
                    const series = this.axis.series[0];
                    const point = series.points.find(p => p.x === this.value);
                    return point?.realTime
                        ? Highcharts.dateFormat('%d-%m %H:%M', point.realTime)
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
            borderRadius: 8
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
    async function getStatus() {
        //TODO: will update these with live SignalR connection
        try {
            await new Promise(resolve => setTimeout(resolve, 500)); // Brief delay to allow server processing - will be removed with SignalR
            const response = await authorizedFetch(statusUrl);
            if (!response.ok) throw new Error('Network response was not ok');
            const state = (await response.json()).state;
            statusEl.querySelector('span').textContent = state.status;
            statusDot.className = state.status === 'Running' ? 'status-dot running' : state.status === 'Paused' ? 'status-dot paused' : 'status-dot stopped';
            statusEl.className = state.status === 'Running' ? 'status running' : state.status === 'Paused' ? 'status paused' : 'status stopped';
        } catch (error) {
            console.error('Error fetching status:', error);
        }
    }

    getStatus();

    playBtn.addEventListener('click', () => {
        try {
            authorizedFetch(playUrl, { method: 'POST' });
        } catch (error) {
            console.error('Error starting replay:', error);
        }
        getStatus();
    });

    pauseBtn.addEventListener('click', () => {
        try {
            authorizedFetch(pauseUrl, { method: 'POST' });
        } catch (error) {
            console.error('Error pausing replay:', error);
        }
        getStatus();
    });

    stopBtn.addEventListener('click', () => {
        try {
            authorizedFetch(stopUrl, { method: 'POST' });
        } catch (error) {
            console.error('Error stopping replay:', error);
        }
        getStatus();
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

            newData.sort((a, b) => a.realTime - b.realTime);
            let barInx = 0;
            newData.forEach(point => {
                point.x = barInx++;
            });
            series.setData(newData, true);

            // Update stats
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

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    setInterval(fetchChartData, 5000); //TODO: will replace with SignalR live updates

    //////

    // Test SignalR

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/notificationHub", {
            accessTokenFactory: () => jwtToken,
            transport: signalR.HttpTransportType.LongPolling // Websockets causing Docker issues locally
        })
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveMessage", (user, message) => {
        console.log(`Message from ${user}: ${message}`);
    });

    connection.start()
        .then(() => {
            console.log("SignalR Connected.");
        })
        .catch(err => console.error("Error connecting to SignalR: ", err));

    //////
});
