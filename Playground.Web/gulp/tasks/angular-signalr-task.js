var gulp = require('gulp');

gulp.task('copyAngularSignalR', function () {
    gulp.src('bower_components/angular-signalr-hub/signalr*.js')
        .pipe(gulp.dest('lib/angular-signalr'));
});