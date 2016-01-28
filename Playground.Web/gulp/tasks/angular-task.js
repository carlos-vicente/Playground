var gulp = require('gulp');

gulp.task('copyAngular', function () {
    gulp.src('bower_components/angular/angular*.js')
        .pipe(gulp.dest('lib/angular'));
});