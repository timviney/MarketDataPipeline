document.addEventListener('DOMContentLoaded', function () {

    // Initialize the chart
    const chart = Highcharts.chart('container', {
        title: { text: 'Simulated Market Ticks' },
        xAxis: {
            type: 'linear',
            labels: {
                formatter: function () {
                    const series = this.axis.series[0];
                    const point = series.points.find(p => p.x === this.value);
                    return point?.realTime
                        ? Highcharts.dateFormat('%d-%m %H:%M', point.realTime)
                        : '';
                }
            }
        },
        yAxis: { title: { text: 'Price GBp' } },
        series: [{
            name: 'BARC Closing Price',
            data: []
        }]
    });

    const apiUrl = 'http://localhost:5001/symbols/BARC/calculations';

    async function fetchData() {
        try {
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Network response was not ok');

            // The data is the Dictionary<DateTime, TickCalculations> object
            const dictionaryData = await response.json();

            const series = chart.series[0];
            const newData = [];


            for (const dateTimeKey in dictionaryData) {
                if (dictionaryData.hasOwnProperty(dateTimeKey)) {

                    // Get the TickCalculations object for this time
                    const calculations = dictionaryData[dateTimeKey];

                    const t = new Date(dateTimeKey).getTime();
                    const sma = calculations.dailySma;
                    const closingPrice = calculations.tick.close;

                    newData.push({ y: closingPrice, realTime: t, sma: sma });
                }
            }

            // This is suboptimal - we should ideally avoid re-sorting and re-assigning x values on every fetch
            newData.sort((a, b) => a.realTime - b.realTime);
            let barInx = 0;
            newData.forEach(point => {
                point.x = barInx++;
            });
            series.setData(newData, true);

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    // Poll every 1 second
    setInterval(fetchData, 1000);
});