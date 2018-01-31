/**
 * 
 */

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

function GetJson(uri,handler) {

	var req = createRequest(); // defined above
	// Create the callback:
	req.onreadystatechange = function() {
	  if (req.readyState != 4) return; // Not there yet
	  if (req.status != 200) {
	    return;
	  }
	  // Request successful, read the response
	  var resp = req.responseText;
	  handler(JSON.parse(resp));
	}
	
	req.open("GET",uri,true);
	req.send();
	return;
}


function jsonSerialze() {
	var json = {};
	json['Username'] = document.getElementById("uname").value;
	json['Password'] = document.getElementById("passw").value;
	
	alert (JSON.stringify(json));
}

function getToken(){
	return 	localStorage.getItem("Token");
}

function submitForm(url) {
	var json = {};
	json['_id'] = document.getElementById("uname").value;
	json['Password'] = document.getElementById("passw").value;
	var req = createRequest();
	req.onreadystatechange = function() {
		  if (req.readyState != 4) return; // Not there yet
		  if (req.status != 200) {
			alert("error :("+req.statusText);
		    return;
		  }
		  else
		  {
			  if(this.responseText.trim()=='-1'){
				  document.getElementById("loginstatus").innerHTML = "Username or Password is invalid";
			  }
			  else{
				  if(url=='http://127.0.0.1:5001/UserLogin'){
					localStorage.setItem('Token', this.responseText);
					var token = localStorage.getItem("Token");
					document.getElementById("loginstatus").innerHTML = "Logged is as:" + document.getElementById("uname").value + ". Your Token is: "+ token;
				  }
			  }
		  }
		}
	req.open("POST",url,true);
	req.setRequestHeader("Content-Type","application/json");
	req.send(JSON.stringify(json));
}

function SendPark(url,method) {
	var json = {};
	json['_id'] = document.getElementById("parkid").value;
	json['_rev'] = document.getElementById("parkrev").value;
	json['Name'] = document.getElementById("ParkName").value;
	json['Size'] = document.getElementById("ParkSize").value;
	json['Floors'] = document.getElementById("ParkFloors").value;
	json['Rows'] = document.getElementById("ParkRows").value;
	json['Cells'] = document.getElementById("ParkCells").value;
	json['X'] = document.getElementById("ParkX").value;
	json['Y'] = document.getElementById("ParkY").value;
	var req = createRequest();
	req.onreadystatechange = function() {
		  if (req.readyState != 4) return; // Not there yet
		  if (req.status != 200) {
			alert("error :("+req.statusText);
		    return;
		  }
		  if (this.responseText=='-1'){
			  document.getElementById("parkstatus").innerHTML="Please Login";
		  }
		  else{
			  document.getElementById("parkstatus").innerHTML = "Success "+method;
		  }
		}
	req.open(method,url,true);
	req.setRequestHeader("Content-Type","application/json");
	req.setRequestHeader("Token",getToken());
	req.send(JSON.stringify(json));
}

function fillDropDown(json) {
	var i;
	var rows = json.rows;
	var menu = document.getElementById("picture_scroller");
	var html
	for (i=0; i<rows.length; i++) {
		var node = document.createElement("option");
		var text = document.createTextNode(rows[i].key);
		node.appendChild(text);
		node.setAttribute("id",rows[i].value);
		menu.appendChild(node);
	}
	
	
}

function getParkSpots(url) {
	var json = {};
	var park = document.getElementById("parknameEmpty").value;
	url = url+"/"+park;
	var req = createRequest();
	req.onreadystatechange = function() {
		  if (req.readyState != 4) return; // Not there yet
		  if (req.status != 200) {
			alert("error :("+req.statusText);
		    return;
		  }
		  else
		  {
			  if(this.responseText.trim()=='-1'){
				  document.getElementById("parkEmpties").innerHTML = "There is no such park";
			  }
			  else{
					document.getElementById("parkEmpties").innerHTML = "There is:" + this.responseText + " free spots in "+park;
				  }
		  }
		}
	req.open("GET",url,true);
	req.setRequestHeader("Content-Type","application/json");
	req.send(JSON.stringify(json));
}