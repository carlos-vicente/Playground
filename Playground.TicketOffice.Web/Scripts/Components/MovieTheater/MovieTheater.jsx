var MovieTheater = React.createClass({
    render: function() {
        return (
            <div className="movie-theater">Hello, world! I am a Movie Theater.</div>
        );
    }
});

ReactDOM.render(
    <MovieTheater />,
    document.getElementById('theater')
);