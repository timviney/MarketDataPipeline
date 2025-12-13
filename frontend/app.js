document.addEventListener('DOMContentLoaded', function () {

    // Initialize the chart
    const chart = Highcharts.chart('container', {
        title: { text: 'Simulated Market Ticks' },
        xAxis: { type: 'datetime' },
        yAxis: { title: { text: 'Price GBp' } },
        series: [{
            name: 'BARC Closing Price',
            data: [] // Start empty
        }]
    });

    const apiUrl = 'http://localhost:5001/symbols/BARC/calculations';

    async function fetchData() {
        try {
            console.log('Fetching data from API...');
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Network response was not ok');
            console.log('Fetched data from API...');

            // The data is the Dictionary<DateTime, TickCalculations> object
            const dictionaryData = await response.json();
            console.log('Data received:', dictionaryData);

            const series = chart.series[0];

            for (const dateTimeKey in dictionaryData) {
                if (dictionaryData.hasOwnProperty(dateTimeKey)) {

                    // Get the TickCalculations object for this time
                    const calculations = dictionaryData[dateTimeKey];


                    const t = new Date(dateTimeKey).getTime();
                    const sma = calculations.dailySma;
                    const closingPrice = calculations.tick.close;

                    console.log(`Plotting: Time=${t}, price=${closingPrice}, SMA=${sma}`);

                    series.addPoint([t, closingPrice], true);
                }
            }

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    // Poll every 1 second
    setInterval(fetchData, 1000);
});