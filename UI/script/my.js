function createRequest() {
  var result = null;
  if (window.XMLHttpRequest) {
	    // code for modern browsers
	    result = new XMLHttpRequest();
	 } else {
	    // code for old IE browsers
	    result = new ActiveXObject("Microsoft.XMLHTTP");
   }
  return result;
}

function submitForm() {
	var json = {};
	json['_id'] = document.getElementById("uname").value;
	json['password'] = document.getElementById("passw").value;
	var req = createRequest();
	req.onreadystatechange = function() {
		  if (req.readyState != 4) return; // Not there yet
		  if (req.status != 200) {
			alert("error :("+req.statusText);
		  }
		}
	req.open("POST","http://127.0.0.1/UserLogin",true,5001);
	req.setRequestHeader("Content-Type","application/json");
	req.send(JSON.stringify(json));
}