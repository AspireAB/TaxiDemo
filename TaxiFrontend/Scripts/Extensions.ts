interface Array<T> {
	toggleValue: (value: T) => void;
}
Array.prototype.toggleValue = function <T>(value: T) {
	var index = this.indexOf(value);
	if (index === -1) {
		this.push(value);
	} else {
		this.splice(index, 1);
	}
}