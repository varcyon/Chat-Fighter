<?php
$con = mysqli_connect('localhost','id10714824_chatfightervarcyon','Dragon20Sariou','id10714824_chatfighter');
$channelName = $_POST["channel"];

$streamerCheckQuery = "SELECT channel FROM Streamers WHERE channel='" . $channelName . "' AND platform='Twitch';";

$streamer = mysqli_query($con,$streamerCheckQuery) or die();   

if(mysqli_num_rows($streamer) > 0){
    echo 1;
    exit();
}
else{
    echo 0;
    exit();
}
?>