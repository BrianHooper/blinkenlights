const fs = require('fs');

function copy(source, dest) {
	fs.copyFile(source, dest, (err) => {
		if (err) throw err;
	});
}

fs.mkdirSync("./wwwroot/lib/jquery/dist", { recursive: true });
copy("./node_modules/jquery/dist/jquery.js", "./wwwroot/lib/jquery/dist/jquery.js");