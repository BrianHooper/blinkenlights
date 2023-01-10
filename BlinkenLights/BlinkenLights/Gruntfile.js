module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/grunt/*", "temp/"],
        concat: {
            all: {
                src: ['Scripts/jquery.js'],
                dest: 'temp/combined.js'
            }
        },
        jshint: {
            files: ['temp/*.js'],
            options: {
                '-W069': false,
            }
        },
        uglify: {
            all: {
                src: ['temp/combined.js'],
                dest: 'wwwroot/grunt/combined.min.js'
            }
        },
        watch: {
            files: ["TypeScript/*.js"],
            tasks: ["all"]
        }
    });



    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.registerTask("all", ['clean', 'concat', 'jshint', 'uglify']);
};