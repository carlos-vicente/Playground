var gulp = require('gulp');

// This task makes sure that when running gul
gulp.task('copyReact', function () {
    gulp.src('wwwroot/lib/react/react*.js')
        .pipe(gulp.dest('lib/react'));
});