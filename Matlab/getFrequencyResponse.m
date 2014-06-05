function ach = getFrequencyResponse(channel, F, time)
 K=size(channel,3);
 rays=size(channel,1);
ach = zeros(F,K);
    for k=1:K
        for frec=1:F
            for z=1:rays
                h = channel(z,1,k)+1j*channel(z,2,k);
                ach(frec,k)=ach(frec,k)+h*exp(-1j*2*pi*15000*(frec-1)*time(z)*10^-9);
            end
        end
    end
    
surf(ach(1:F/10,1:K/10).*conj(ach(1:F/10,1:K/10)));

end

