const gulp = require("gulp");
const sass = require("gulp-sass")(require("sass"));
const cssMinify = require("gulp-clean-css");
const uglify = require("gulp-uglify");
const rename = require("gulp-rename");

const cssDst = "./wwwroot/css/";
const jsDst = "./wwwroot/js/";
const fontsDst = "./wwwroot/fonts/";

function processFonts() {
    return gulp
        .src("./node_modules/font-awesome/fonts/**")
        .pipe(gulp.dest(fontsDst));
}

function processSass() {
    return gulp
        .src("./wwwroot/sass/**")
        .pipe(sass())
        .on("error", sass.logError)
        //.pipe(cssMinify())
        .pipe(gulp.dest(cssDst));
}

function processStyles() {
    return gulp
        .src("./node_modules/font-awesome/css/font-awesome.css")
        .pipe(gulp.dest(cssDst));
}

function processScripts() {
    return gulp
        .src(["./node_modules/jquery/dist/jquery.js",
            "./node_modules/jquery-validation/dist/jquery.validate.js",
            "./node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
            "./node_modules/@popperjs/core/dist/umd/popper.js",
            "./node_modules/bootstrap/dist/js/bootstrap.js"])
        .pipe(gulp.dest(jsDst))
        .pipe(uglify())
        .pipe(rename({extname: ".min.js"}))
        .pipe(gulp.dest(jsDst));
}

const all = gulp.parallel(processScripts, gulp.series(processFonts, processSass, processStyles));

gulp.task("sass", processSass);
gulp.task("styles", processStyles);
gulp.task("scripts", processScripts);
gulp.task("all", all);
