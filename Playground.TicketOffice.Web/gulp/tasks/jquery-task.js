var gulp = require('gulp');

// This task makes sure that when running gul
gulp.task('copyJquery', function(){
    gulp.src('wwwroot/lib/jquery/dist/*.js')
        .pipe(gulp.dest('lib/jquery'));
});