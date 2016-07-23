var gulp = require('gulp');

gulp.task('copyMaterialize', function(){
    gulp.src('wwwroot/lib/Materialize/dist/**/*.*')
        .pipe(gulp.dest('lib/materialize/'));
});