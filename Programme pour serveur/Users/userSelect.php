<?php
include('connection.php');
//echo "oui je suis bien rentré";

if (isset($_POST["username"]) && !empty($_POST["username"])&& isset($_POST["password"]) && !empty($_POST["password"]))
	{

	//echo $_POST["username"],$_POST["password"], sha1($_POST["password"]);
	$sql = "select ID , Username , Password from Authentification WHERE Username = (?) AND Password = (?) ";
	$st = $connect->prepare($sql);
	$st->bind_param("ss",$username, $password);
	$username = sha1($_POST["username"]);
	$password = sha1($_POST["password"]);
	$st->execute();
	$st->bind_result($ID,$Username,$Password);
	$st->fetch();
	$st -> close();

	if (	$ID != null)
	{
		//echo "  On revient à la ligne \n";
    	//echo ($ID." ".$Username." ".$Password.";");
    	$sql = "select * from Inventory WHERE Username = (?)";
    	$st = $connect->prepare($sql);
    	$st->bind_param("s",$username);
		$username = sha1($_POST["username"]);
		$st->execute();
		$result = $st->get_result();
		while($row = $result->fetch_assoc())
		{
			echo $row['health'].";";
			echo $row['defense'].";";
			echo $row['speed'].";";

		}
		$st->fetch();
		$st -> close(); 

    	exit();
  	}
	else {
		echo " Authentification Error";
		exit();

  	}
}
else {

	echo "Error Serveur";
	exit();
}

?>

