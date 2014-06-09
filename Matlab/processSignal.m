function result = processSignal(signal, channel, tau, Td)
    rays =size(channel,1);
    len = min([length(signal),length(channel),1000]);
    start = max(round((tau()*10^-1)/3.2552))+1;
    result = zeros(len,1);
    channelK = 1;
    step = 100;
    for k=start:len  
        for z = 1:rays
            Hz =  channel(z,1,channelK)+1j*channel(z,2,channelK);
            
            result(k-start+1) = result(k-start+1)+Hz*signal(k-round((tau(z)*10^-1)/3.2552));
        end
        if mod(k-1,step)==0
            channelK = channelK+1;
        end
    end
end

