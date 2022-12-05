
function GetAduioData(json) {

    const params = {
        waveColor: 'blue',
        progressColor: 'purple',
        barHeight: 1,
        barWidth: 1,
        splitChannels: true
    };

    const wavesurfer = WaveSurfer.create(params);
    wavesurfer.loadBlob(new window.Blob([new Uint8Array(JSON.parse(json))]));

    return wavesurfer.exportImage('image/jpeg', 1, 'blob');
}