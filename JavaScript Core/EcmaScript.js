function Func1(str) {

	// Search of digits and signs of arithmetical operations
	var regexp = /\d+(?:\.\d*)?|[\+\-\*\/]/g;
	var array = str.match(regexp);

	var result = 0;
	var operation = "+"

	// Evaluate expression
	for (var i = 0; i < array.length; i++) {

		if (isNaN(array[i]) == false) {
			switch (operation) {
				case "+": result = result + Number(array[i]); break;
				case "-": result = result - Number(array[i]); break;
				case "*": result = result * Number(array[i]); break;
				case "/": result = result / Number(array[i]); break;
			}
		}
		else {
			operation = array[i];
		}
	}

	return result.toFixed(2);
}

function Func2(str) {

	// Split the string on separate words
	var regexp = /[\s\.\?\,\;\:\!]/;
	var wordsArray = str.split(regexp);
	
	if (wordsArray[wordsArray.length - 1] == "")
		wordsArray.pop();

	var charsArray = [];
	
	// Search of all equal chars in words
	for (var i = 0; i < wordsArray.length - 1; i++) {

		var word1 = wordsArray[i].toLowerCase();
		var word2 = wordsArray[i + 1].toLowerCase();

		for (var char1 in word1) {
			var symbol = word1[char1];
			for (var char2 in word2) {
				if (word2[char2] == symbol && charsArray.indexOf(symbol) == -1) {
					charsArray.push(symbol);
					break;
				}
			}
		}
	}
	
	if (charsArray.length == 0)
		return str;
	
	// Filtering of chars (Exclude chars which are not presented in each word)
	for (var i = 0; i < wordsArray.length; i++) {
		var word = wordsArray[i].toLowerCase();

		for (var j = 0; j < charsArray.length; j++) {
			var char = charsArray[j];

			if (word.indexOf(char) == -1) {
				charsArray.splice(j, 1);
				j--;
			}

		}
	}

	// Replace chars in words
	for (var index in charsArray) {
		str = str.replace(new RegExp("\\"+charsArray[index],"ig"),"");
	}

	return str;
}

str1 = "3.5 землекопа +4 поросенка *10 рублей - 5.5 $ /5 человек ="
str2 = "Чего-С изволите-с?Барин-С!"

alert("Исходная строка: " + str1 + "\r\nРезультат: " +Func1(str1));
alert("Исходная строка: " + str2 + "\r\nРезультат: " + Func2(str2));
