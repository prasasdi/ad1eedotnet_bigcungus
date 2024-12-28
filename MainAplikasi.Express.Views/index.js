const express = require('express');
const fs = require('fs');
const path = require('path');
const app = express();
const port = 3030;

// Serve static files
app.use(express.static('public'));

// Helper function to read HTML files and replace placeholders
function render(templateName, options) {
    const filePath = path.join(__dirname, 'views', templateName);
    let template = fs.readFileSync(filePath, 'utf-8');

    // Replace {{title}} and {{{body}}} in layout.html
    if (templateName === '/layout/layout.html') {
        template = template.replace('{{title}}', options.title);
        template = template.replace('{{{body}}}', options.body);
        
        template = template.replace('{{context}}', options.context);
        template = template.replace('{{context}}', options.context);
    }

    return template;
}

// Route for the homepage
app.get('/', (req, res) => {
    const body = fs.readFileSync(path.join(__dirname, 'views', 'index.html'), 'utf-8');
    const html = render('/layout/layout.html', { title: 'Home', body , context: 'home'});
    res.send(html);
});

// Start the server
app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});
