document.addEventListener('DOMContentLoaded', function () {

    // Initialize the chart
    const chart = Highcharts.chart('container', {
        title: { text: 'Simulated Market Ticks' },
        xAxis: { type: 'datetime' },
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

                    newData.push([t, closingPrice]);
                }
            }

            newData.sort((a, b) => a[0] - b[0]);
            series.setData(newData, true);

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    // Poll every 1 second
    setInterval(fetchData, 1000);
});