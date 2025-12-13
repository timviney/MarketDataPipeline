document.addEventListener('DOMContentLoaded', function () {
    
    // Initialize the chart
    const chart = Highcharts.chart('container', {
        title: { text: 'Simulated Market Ticks' },
        xAxis: { type: 'datetime' },
        yAxis: { title: { text: 'Price / SMA' } },
        series: [{
            name: 'SMA Value',
            data: [] // Start empty
        }]
    });

    const apiUrl = 'http://localhost:5001/symbols/BARC/calculations';

    async function fetchData() {
        try {
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Network response was not ok');
            
            const data = await response.json();
            
            // Assuming your API returns object like: { time: "2023-...", value: 102.5 }
            // Highcharts wants [timestamp, value]
            
            // Adjust this parsing based on your actual C# DTO structure
            const x = new Date(data.time).getTime(); 
            const y = data.value; 

            const series = chart.series[0];
            
            // Add point. (true = redraw, true = shift if > 20 points)
            const shift = series.data.length > 20; 
            series.addPoint([x, y], true, shift);

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    // Poll every 1 second
    setInterval(fetchData, 1000);
});