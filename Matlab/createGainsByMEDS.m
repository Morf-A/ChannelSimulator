function c = createGainsByMEDS(profile, sinusoidsNumber)
    rays = length(profile);
    c = zeros(2,sinusoidsNumber+1,rays);
    for i=1:2
        for z=1:rays
            for n = 1: sinusoidsNumber+1
                c(i,n,z) = profile(z)*sqrt(2/sinusoidsNumber);
            end
        end
        sinusoidsNumber=sinusoidsNumber+1;
    end
end

