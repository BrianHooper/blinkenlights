const fs = require('fs');

function copy(source, dest) {
	fs.copyFile(source, dest, (err) => {
		if (err) throw err;
	});
}

//fs.mkdirSync("./wwwroot/lib/leaflet", { recursive: true });
//copy("./node_modules/leaflet/dist/*", "./wwwroot/lib/leaflet");

fs.mkdirSync("./wwwroot/lib/jquery/dist", { recursive: true });
copy("./node_modules/jquery/dist/jquery.js", "./wwwroot/lib/jquery/dist/jquery.js");

fs.mkdirSync("./wwwroot/lib/d3/dist", { recursive: true });
copy("./node_modules/d3/dist/d3.js", "./wwwroot/lib/d3/dist/d3.js");