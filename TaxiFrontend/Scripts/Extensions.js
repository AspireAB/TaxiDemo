Array.prototype.toggleValue = function (value) {
    var index = this.indexOf(value);
    if (index === -1) {
        this.push(value);
    }
    else {
        this.splice(index, 1);
    }
};
