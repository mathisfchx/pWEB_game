<?php
include('connection.php');
if (isset($_POST["username"]) && !empty($_POST["username"])&& isset($_POST["password"]) && !empty($_POST["password"]))
{
	$sql = "select username , password , connected_state  from authentification WHERE username = (?) AND password = (?) ";
	$st = $connect->prepare($sql);
	$st->bind_param("ss",$username, $password);
	$username = $_POST["username"];
	$password = hash('sha512',$_POST['username'].$_POST['password'].getenv("SERVER_KEY"));
	$st->execute();
	$st->bind_result($Username,$Password,$Connected_state);
	$st->fetch();
	$st -> close();
	if ((int)$Connected_state != 1 && $Username !=null )
	{	
    		$sql = "update authentification set connected_state = (?) WHERE username = (?)";
    		$st = $connect->prepare($sql); 
    		$st->bind_param("is",$connected_state,$username);
    		$connected_state = 1 ; 
    		$username = $_POST["username"];
    		$st->execute();
    		$st->fetch();
    		$st-> close();
  		$sql = "select * from characters WHERE username = (?)"; //;select Encryption from authentification WHERE Username = (?)
    		$st = $connect->prepare($sql);
  		$st->bind_param("s",$username);
		$username = $_POST["username"];
		$st->execute();
		$result = $st->get_result();
		while($row = $result->fetch_assoc())
		{	
			echo $row['username']."&";
			echo $row['level']."&";
			echo $row['experience']."&";
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

