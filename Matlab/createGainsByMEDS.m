function c = createGainsByMEDS(profile, sinusoidsNumber)
    rays = length(profile);
    c = zeros(2,rays);
    for i=1:2
        for z=1:rays
            c(i,z) = profile(z)*sqrt(2/sinusoidsNumber);
        end
        sinusoidsNumber=sinusoidsNumber+1;
    end
end

